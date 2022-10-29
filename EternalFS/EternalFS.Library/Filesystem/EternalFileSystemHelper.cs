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
        using Stream stream = fileSystem.GetStream();

        stream.Seek(EternalFileSystemHeader.HeaderSize, SeekOrigin.Begin);

        ushort clustersCount = fileSystem.ClustersCount;

        for (ushort i = 0; i < clustersCount; i++)
        {
            entry = stream.MarshalReadStructure<EternalFileSystemFatEntry>();

            if (entry != EternalFileSystemMounter.EmptyCluster)
                continue;

            entry = i;

            stream.Seek(-EternalFileSystemFatEntry.EntrySize, SeekOrigin.Current);
            stream.MarshalWriteStructure(EternalFileSystemMounter.FatTerminator);

            return true;
        }

        entry = default;
        return false;
    }

    public static int GetClusterOffset(EternalFileSystem fileSystem, EternalFileSystemFatEntry entry)
    {
        return EternalFileSystemHeader.HeaderSize +
            fileSystem.ClustersCount * EternalFileSystemFatEntry.EntrySize +
            entry * EternalFileSystemMounter.CLUSTER_SIZE_BYTES;
    }

    public static int GetFatEntryOffset(EternalFileSystemFatEntry entry)
    {
        return EternalFileSystemHeader.HeaderSize +
            entry * EternalFileSystemFatEntry.EntrySize;
    }
}
