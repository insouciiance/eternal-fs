using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem;

public static class EternalFileSystemHelper
{
    public static bool TryLocateSubEntry(
        EternalFileSystem fileSystem,
        EternalFileSystemFatEntry directory,
        in ReadOnlySpan<byte> subEntryName,
        out EternalFileSystemEntry entry)
    {
        using EternalFileSystemFileStream stream = new(fileSystem, directory);

        int entriesCount = stream.MarshalReadStructure<int>();

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

            if (currentEntry.SubEntryName.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(subEntryName))
            {
                entry = currentEntry;
                return true;
            }
        }

        entry = default;
        return false;
    }
    
    public static bool TryAllocateNewFatEntry(EternalFileSystem fileSystem, out EternalFileSystemFatEntry entry)
    {
        HashSet<EternalFileSystemFatEntry> occupiedEntries = new() { EternalFileSystem.RootDirectoryEntry };

        TraverseDirectory(EternalFileSystem.RootDirectoryEntry);

        ushort clustersCount = fileSystem.ClustersCount;

        for (ushort i = 0; i < clustersCount; i++)
        {
            if (occupiedEntries.Contains(i))
                continue;

            entry = i;
            return true;
        }

        entry = default;
        return false;

        void TraverseDirectory(EternalFileSystemFatEntry directoryEntry)
        {
            using var stream = new EternalFileSystemFileStream(fileSystem, directoryEntry);

            int entriesCount = stream.MarshalReadStructure<int>();

            for (int i = 0; i < entriesCount; i++)
            {
                var entry = stream.MarshalReadStructure<EternalFileSystemEntry>();

                if (entry.IsDirectory && !occupiedEntries.Contains(entry.FatEntryReference))
                {
                    occupiedEntries.Add(entry.FatEntryReference);
                    TraverseDirectory(entry.FatEntryReference);
                    continue;
                }

                RecordFileChain(entry.FatEntryReference);
            }

            void RecordFileChain(EternalFileSystemFatEntry fileEntry)
            {
                occupiedEntries.Add(fileEntry);

                Stream fsStream = fileSystem.GetStream();
                fsStream.Seek(EternalFileSystemHeader.HeaderSize + 2 + Marshal.SizeOf<EternalFileSystemFatEntry>() * fileEntry, SeekOrigin.Begin);
                var nextEntry = fsStream.MarshalReadStructure<EternalFileSystemFatEntry>();
                fsStream.Dispose();

                if (nextEntry != EternalFileSystemMounter.FatTerminator)
                    RecordFileChain(nextEntry);
            }
        }
    }
}
