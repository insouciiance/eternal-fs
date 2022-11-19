using System;
using System.Diagnostics.CodeAnalysis;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Library.Filesystem.Accessors;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> that also uses
/// <see cref="IEntryIndexer"/> internally to index file system entries.
/// </summary>
public class EternalFileSystemIndexerAccessor : IEternalFileSystemAccessor
{
    public required IEternalFileSystemAccessor Accessor { get; init; }

    public required IEntryIndexer Indexer { get; init; }

    [SetsRequiredMembers]
    public EternalFileSystemIndexerAccessor(IEternalFileSystemAccessor accessor, IEntryIndexer indexer)
    {
        Accessor = accessor;
        Indexer = indexer;
    }

    public void Initialize(EternalFileSystem fileSystem)
    {
        Accessor.Initialize(fileSystem);
        Indexer.Initialize(fileSystem);
    }

    public EternalFileSystemEntry LocateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName)
    {
        if (Indexer.TryLocateEntry(directoryEntry, subEntryName, out var subEntry))
            return subEntry;

        return Accessor.LocateSubEntry(directoryEntry, subEntryName);
    }

    public EternalFileSystemEntry CreateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName, bool isDirectory)
    {
        var entry = Accessor.CreateSubEntry(directoryEntry, subEntryName, isDirectory);
        Indexer.RecordChange(directoryEntry, subEntryName, EntryChangeKind.Add);
        return entry;
    }

    public void DeleteSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName)
    {
        Accessor.DeleteSubEntry(directoryEntry, fileName);
        Indexer.RecordChange(directoryEntry, fileName, EntryChangeKind.Remove);
    }

    public void WriteFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName, in ReadOnlySpan<byte> content)
    {
        Accessor.WriteFile(directoryEntry, fileName, content);
        Indexer.RecordChange(directoryEntry, fileName, EntryChangeKind.Modify);
    }

    public void CopySubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to)
    {
        Accessor.CopySubEntry(directoryEntry, from, to);
        Indexer.RecordChange(directoryEntry, to, EntryChangeKind.Add);
    }
}
