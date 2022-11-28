﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EternalFS.Library.Commands;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Indexing;

/// <summary>
/// A simple <see cref="IEntryIndexer"/> that uses <see cref="Dictionary{TKey, TValue}"/> internally to index values.
/// </summary>
public class DictionaryEntryIndexer : IEntryIndexer
{
    private readonly Dictionary<string, EternalFileSystemEntry> _entriesCache = new();

    private EternalFileSystem _fileSystem = null!;

    public void Initialize(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _entriesCache.Clear();
        IndexFileSystem();
    }

    public bool TryLocateEntry(in SubEntryInfo info, out EternalFileSystemEntry entry)
    {
        return _entriesCache.TryGetValue(info.Name.GetString(), out entry);
    }

    public void RecordChange(in SubEntryInfo info, EntryChangeKind changeKind)
    {
        string entryName = GetEntryName(info);

        if (changeKind == EntryChangeKind.Remove)
        {
            _entriesCache.Remove(entryName);
            return;
        }

        var manager = new EternalFileSystemManager();
        manager.Initialize(_fileSystem);

        var subEntry = manager.LocateSubEntry(info);

        if (changeKind == EntryChangeKind.Add)
        {
            _entriesCache.Add(entryName, subEntry);
            return;
        }

        _entriesCache[entryName] = subEntry;
    }

    [Conditional("DEBUG")]
    internal void WriteInternalIndex(ref CommandExecutionContext context)
    {
        context.Writer.AppendLine("\nInternal index map:");

        var index = GetInternalIndex();

        foreach (var (key, value) in index)
            context.Writer.AppendLine($$"""{ {{key}}, {{value}} }""");

        Dictionary<string, string> GetInternalIndex()
        {
            return _entriesCache.ToDictionary(kvp => kvp.Key, kvp => ((ReadOnlySpan<byte>)kvp.Value.SubEntryName).GetString());
        }
    }

    private void IndexFileSystem()
    {
        List<string> directoryStack = new()
        {
            EternalFileSystemMounter.ROOT_DIRECTORY_NAME
        };
        
        IndexDirectory(EternalFileSystemMounter.RootDirectoryEntry);

        void IndexDirectory(EternalFileSystemFatEntry directoryEntry)
        {
            using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

            int entriesCount = stream.MarshalReadStructure<int>();

            for(int i = 0; i < entriesCount; i++)
            {
                var entry = stream.MarshalReadStructure<EternalFileSystemEntry>();
                _entriesCache.Add(GetEntryName(new(directoryEntry, entry.SubEntryName)), entry);

                if (!entry.IsDirectory)
	                continue;

                ReadOnlySpan<byte> subDirectoryName = entry.SubEntryName;
                subDirectoryName = subDirectoryName.TrimEndNull();
                    
                // skip parent directory and this directory, they have already been indexed.
                if (subDirectoryName.SequenceEqual(ByteSpanHelper.ParentDirectory()) ||
                    subDirectoryName.SequenceEqual(ByteSpanHelper.Period()))
	                continue;
                    
                directoryStack.Add(subDirectoryName.GetString());
                    
                IndexDirectory(entry.FatEntryReference);
            }

            directoryStack.RemoveAt(directoryStack.Count - 1);
        }
    }

    private static string GetEntryName(in SubEntryInfo info)
        => $"{(ushort)info.FatEntry}_{info.Name.GetString()}";
}
