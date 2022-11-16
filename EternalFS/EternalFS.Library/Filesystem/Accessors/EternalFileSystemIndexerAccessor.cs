using System;
using System.Collections.Generic;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Library.Filesystem.Accessors;

public class EternalFileSystemIndexerAccessor : IEternalFileSystemAccessor
{
    private readonly EternalFileSystemManager _manager;

    private readonly IEntryIndexer _indexer;

    public EternalFileSystemIndexerAccessor(EternalFileSystemManager manager, IEntryIndexer indexer)
    {
        _manager = manager;
        _indexer = indexer;
    }

    public void Initialize(EternalFileSystem fileSystem)
    {
        _manager.Initialize(fileSystem);
        _indexer.Initialize(fileSystem);
    }

    public EternalFileSystemEntry LocateDirectory(ICollection<string> directoryStack)
    {
        if (_indexer.TryLocateDirectory(directoryStack, out var entry))
            return entry;

        return _manager.LocateDirectory(directoryStack);
    }

    public EternalFileSystemEntry LocateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName)
    {
        if (_indexer.TryLocateEntry(directoryEntry, subEntryName, out var subEntry))
            return subEntry;

        return _manager.LocateSubEntry(directoryEntry, subEntryName);
    }

    public EternalFileSystemEntry CreateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName, bool isDirectory)
    {
        var entry = _manager.CreateSubEntry(directoryEntry, subEntryName, isDirectory);
        _indexer.RecordChange(directoryEntry, subEntryName, EntryChangeKind.Add);
        return entry;
    }

    public void DeleteSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName)
    {
        _manager.DeleteSubEntry(directoryEntry, fileName);
        _indexer.RecordChange(directoryEntry, fileName, EntryChangeKind.Remove);
    }

    public void WriteFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName, in ReadOnlySpan<byte> content)
    {
        _manager.WriteFile(directoryEntry, fileName, content);
        _indexer.RecordChange(directoryEntry, fileName, EntryChangeKind.Modify);
    }

    public void CopyFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to)
    {
        _manager.CopyFile(directoryEntry, from, to);
        _indexer.RecordChange(directoryEntry, to, EntryChangeKind.Add);
    }
}
