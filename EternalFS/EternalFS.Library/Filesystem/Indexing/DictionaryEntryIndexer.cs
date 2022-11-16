using System;
using System.Collections.Generic;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Filesystem.Indexing;

public class DictionaryEntryIndexer : IEntryIndexer
{
    private readonly Dictionary<string, EternalFileSystemEntry> _entriesCache = new();

    private EternalFileSystem _fileSystem = null!;

    public void Initialize(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public bool TryLocateDirectory(ICollection<string> directoryStack, out EternalFileSystemEntry entry)
    {
        throw new NotImplementedException();
    }

    public bool TryLocateEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entryName, out EternalFileSystemEntry entry)
    {
        return _entriesCache.TryGetValue(entryName.GetString(), out entry);
    }

    public void RecordChange(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entrySpan, EntryChangeKind changeKind)
    {
        string entryName = GetEntryName(directoryEntry, entrySpan);

        if (changeKind == EntryChangeKind.Remove)
        {
            _entriesCache.Remove(entryName);
            return;
        }

        var manager = new EternalFileSystemManager();
        manager.Initialize(_fileSystem);

        var subEntry = manager.LocateSubEntry(directoryEntry, entrySpan);

        if (changeKind == EntryChangeKind.Add)
        {
            _entriesCache.Add(entryName, subEntry);
            return;
        }

        _entriesCache[entryName] = subEntry;
    }

    private string GetEntryName(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entryName)
        => $"{(ushort)directoryEntry}_{entryName.GetString()}";
}
