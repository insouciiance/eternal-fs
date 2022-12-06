using System;
using System.IO;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Provides helper methods to operate on <see cref="EternalFileSystem"/>.
/// </summary>
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
            ReadOnlySpan<byte> entryName = currentEntry.SubEntryName;

            if (entryName.TrimEndNull().SequenceEqual(subEntryName))
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

        int clustersCount = fileSystem.ClustersCount;

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

    public static int GetClustersCount(long size)
    {
        return (int)(size / (EternalFileSystemFatEntry.EntrySize + EternalFileSystemMounter.CLUSTER_SIZE_BYTES));
    }

    public static int GetClusterOffset(long size, EternalFileSystemFatEntry entry)
    {
        return EternalFileSystemHeader.HeaderSize +
            GetClustersCount(size) * EternalFileSystemFatEntry.EntrySize +
            entry * EternalFileSystemMounter.CLUSTER_SIZE_BYTES;
    }

    public static int GetFatEntryOffset(EternalFileSystemFatEntry entry)
    {
        return EternalFileSystemHeader.HeaderSize +
            entry * EternalFileSystemFatEntry.EntrySize;
    }
}
