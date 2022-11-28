using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Library.Filesystem.Accessors.Decorators;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> that also uses
/// <see cref="IEntryIndexer"/> internally to index file system entries.
/// </summary>
public class EternalFileSystemIndexerAccessorDecorator : EternalFileSystemAccessorDecorator
{
    public required IEntryIndexer Indexer { get; init; }

    [SetsRequiredMembers]
    public EternalFileSystemIndexerAccessorDecorator(IEternalFileSystemAccessor accessor, IEntryIndexer indexer)
        : base(accessor)
    {
        Indexer = indexer;
    }

    public override void Initialize(EternalFileSystem fileSystem)
    {
        base.Initialize(fileSystem);
        Indexer.Initialize(fileSystem);
    }

    public override EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info)
    {
        if (Indexer.TryLocateEntry(info, out var subEntry))
            return subEntry;

        return Accessor.LocateSubEntry(info);
    }

    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        var entry = Accessor.CreateSubEntry(info, isDirectory);
        Indexer.RecordChange(info, EntryChangeKind.Add);
        return entry;
    }

    public override void DeleteSubEntry(in SubEntryInfo info)
    {
        Accessor.DeleteSubEntry(info);
        Indexer.RecordChange(info, EntryChangeKind.Remove);
    }

    public override void WriteFile(in SubEntryInfo info, Stream source)
    {
        Accessor.WriteFile(info, source);
        Indexer.RecordChange(info, EntryChangeKind.Modify);
    }

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        Accessor.CopySubEntry(from, to);
        Indexer.RecordChange(to, EntryChangeKind.Add);
    }
}
