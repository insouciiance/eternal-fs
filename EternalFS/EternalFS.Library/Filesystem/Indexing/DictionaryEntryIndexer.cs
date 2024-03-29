﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using EternalFS.Library.Collections;
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

    private readonly ListDictionary<EternalFileSystemFatEntry, EternalFileSystemEntry> _subEntriesCache = new();

    private EternalFileSystem _fileSystem = null!;

    public void Initialize(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _entriesCache.Clear();
        _subEntriesCache.Clear();
        IndexFileSystem();
    }

    public bool TryLocateEntry(in SubEntryInfo info, out EternalFileSystemEntry entry)
    {
        return _entriesCache.TryGetValue(GetEntryName(info), out entry);
    }

    public void RecordChange(in SubEntryInfo info, EntryChangeKind changeKind)
    {
        string entryName = GetEntryName(info);

        if (changeKind == EntryChangeKind.Remove)
        {
            _entriesCache.Remove(entryName);
            _subEntriesCache.Remove(info.FatEntry);
            return;
        }

        var manager = new EternalFileSystemManager();
        manager.Initialize(_fileSystem);

        var subEntry = manager.LocateSubEntry(info);

        if (changeKind == EntryChangeKind.Add)
        {
            _entriesCache.Add(entryName, subEntry);
            _subEntriesCache.Add(info.FatEntry, subEntry);
            return;
        }

        _entriesCache[entryName] = subEntry;

        var index = _subEntriesCache[info.FatEntry].FindIndex(e => e.SubEntryName.SequenceEqual(subEntry.SubEntryName));
        _subEntriesCache[info.FatEntry][index] = subEntry;
    }

    public bool TryEnumerateEntries(EternalFileSystemFatEntry directory, SearchOption searchOption, [MaybeNullWhen(false)] out IEnumerable<EternalFileSystemEntry> entries)
    {
        if (_subEntriesCache.TryGetValue(directory, out var list))
        {
            entries = list;
            return true;
        }

        entries = null;
        return false;
    }

#if DEBUG
    public Dictionary<string, string> GetInternalIndex()
    {
        return _entriesCache.ToDictionary(kvp => kvp.Key, kvp => ((ReadOnlySpan<byte>)kvp.Value.SubEntryName).GetString());
    }
#endif

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

            for (int i = 0; i < entriesCount; i++)
            {
                var entry = stream.MarshalReadStructure<EternalFileSystemEntry>();

                ReadOnlySpan<byte> subEntryName = entry.SubEntryName;
                subEntryName = subEntryName.TrimEndNull();

                _entriesCache.Add(GetEntryName(new(directoryEntry, subEntryName)), entry);
                _subEntriesCache.Add(directoryEntry, entry);

                if (!entry.IsDirectory)
                    continue;

                // skip parent directory and this directory, they have already been indexed.
                if (subEntryName.SequenceEqual(ByteSpanHelper.ParentDirectory()) ||
                    subEntryName.SequenceEqual(ByteSpanHelper.Period()))
                    continue;

                directoryStack.Add(subEntryName.GetString());

                IndexDirectory(entry.FatEntryReference);
            }

            directoryStack.RemoveAt(directoryStack.Count - 1);
        }
    }

    private static string GetEntryName(in SubEntryInfo info)
    {
        string entryName = $"{(ushort)info.FatEntry}_{info.Name.GetString()}";
        return entryName;
    }
}
