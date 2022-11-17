using System;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Library.Filesystem.Accessors;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> that also uses
/// <see cref="IEntryIndexer"/> internally to index file system entries.
/// </summary>
public class EternalFileSystemIndexerAccessor : IEternalFileSystemAccessor
{
    private readonly IEternalFileSystemAccessor _accessor;

    private readonly IEntryIndexer _indexer;

    public EternalFileSystemIndexerAccessor(IEternalFileSystemAccessor accessor, IEntryIndexer indexer)
    {
        _accessor = accessor;
        _indexer = indexer;
    }

    public void Initialize(EternalFileSystem fileSystem)
    {
        _accessor.Initialize(fileSystem);
        _indexer.Initialize(fileSystem);
    }

    public EternalFileSystemEntry LocateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName)
    {
        if (_indexer.TryLocateEntry(directoryEntry, subEntryName, out var subEntry))
            return subEntry;

        return _accessor.LocateSubEntry(directoryEntry, subEntryName);
    }

    public EternalFileSystemEntry CreateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName, bool isDirectory)
    {
        var entry = _accessor.CreateSubEntry(directoryEntry, subEntryName, isDirectory);
        _indexer.RecordChange(directoryEntry, subEntryName, EntryChangeKind.Add);
        return entry;
    }

    public void DeleteSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName)
    {
        _accessor.DeleteSubEntry(directoryEntry, fileName);
        _indexer.RecordChange(directoryEntry, fileName, EntryChangeKind.Remove);
    }

    public void WriteFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName, in ReadOnlySpan<byte> content)
    {
        _accessor.WriteFile(directoryEntry, fileName, content);
        _indexer.RecordChange(directoryEntry, fileName, EntryChangeKind.Modify);
    }

    public void CopySubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to)
    {
        _accessor.CopySubEntry(directoryEntry, from, to);
        _indexer.RecordChange(directoryEntry, to, EntryChangeKind.Add);
    }
}
