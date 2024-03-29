﻿using System.Collections.Generic;
using System.IO;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem.Accessors;

public static class EternalFileSystemAccessorHelper
{
    public static IEnumerable<EternalFileSystemEntry> EnumerateEntries(
        IEternalFileSystemAccessor accessor,
        EternalFileSystemFatEntry root,
        SearchOption searchOption)
    {
        List<EternalFileSystemEntry> entries = new();

        EnumerateEntriesInternal(root);

        return entries;

        void EnumerateEntriesInternal(EternalFileSystemFatEntry current)
        {
            using var stream = accessor.OpenEntry(current);

            int entriesCount = stream.MarshalReadStructure<int>();

            for (int i = 0; i < entriesCount; i++)
            {
                var subEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();
                entries.Add(subEntry);

                if (searchOption == SearchOption.AllDirectories && subEntry.IsDirectory && subEntry.IsCommonSubDirectory())
                    EnumerateEntriesInternal(subEntry.FatEntryReference);
            }
        }
    }

    public static DiskUsageInfo GetDiskUsageInfo(IEternalFileSystemAccessor accessor)
    {
        int bytesAllocatedUsed = 0;
        int bytesAllocatedTotal = EternalFileSystemMounter.CLUSTER_SIZE_BYTES; // initial root directory cluster is always there
        int entriesAllocated = 1; // root directory

        foreach (var entry in EnumerateEntries(accessor, EternalFileSystemMounter.RootDirectoryEntry, SearchOption.AllDirectories))
        {
            if (entry.IsDirectory && !entry.IsCommonSubDirectory())
                continue;

            entriesAllocated++;
            
            if (entry.IsDirectory)
            {
                using var stream = accessor.OpenEntry(entry.FatEntryReference);
                
                int entriesCount = stream.MarshalReadStructure<int>();
                int bytesUsed = entriesCount * EternalFileSystemEntry.EntrySize + sizeof(int);
                
                bytesAllocatedUsed += bytesUsed;
                bytesAllocatedTotal += GetTakenClusters(bytesUsed) * EternalFileSystemMounter.CLUSTER_SIZE_BYTES;
                continue;
            }

            bytesAllocatedUsed += entry.Size;
            bytesAllocatedTotal += GetTakenClusters(entry.Size) * EternalFileSystemMounter.CLUSTER_SIZE_BYTES;
        }

        return new(bytesAllocatedTotal, bytesAllocatedUsed, entriesAllocated);

        static int GetTakenClusters(int entrySize)
        {
            if (entrySize == 0)
                return 1; // one cluster is taken already

            int clustersTaken = entrySize / EternalFileSystemMounter.CLUSTER_SIZE_BYTES;

            if (entrySize % EternalFileSystemMounter.CLUSTER_SIZE_BYTES != 0)
                clustersTaken += 1;

            return clustersTaken;
        }
    }
}
