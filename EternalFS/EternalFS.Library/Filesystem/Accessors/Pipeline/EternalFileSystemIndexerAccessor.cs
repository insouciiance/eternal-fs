﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Library.Filesystem.Accessors.Pipeline;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> that also uses
/// <see cref="IEntryIndexer"/> internally to index file system entries.
/// </summary>
public class EternalFileSystemIndexerAccessor : AccessorPipelineElement
{
    public required IEntryIndexer Indexer { get; init; }

    [SetsRequiredMembers]
    public EternalFileSystemIndexerAccessor(IEntryIndexer indexer)
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

        return base.LocateSubEntry(info);
    }

    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        var entry = base.CreateSubEntry(info, isDirectory);
        Indexer.RecordChange(info, EntryChangeKind.Add);
        return entry;
    }

    public override void DeleteSubEntry(in SubEntryInfo info)
    {
        base.DeleteSubEntry(info);
        Indexer.RecordChange(info, EntryChangeKind.Remove);
    }

    public override void WriteFile(in SubEntryInfo info, Stream source)
    {
        base.WriteFile(info, source);
        Indexer.RecordChange(info, EntryChangeKind.Modify);
    }

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        base.CopySubEntry(from, to);
        Indexer.RecordChange(to, EntryChangeKind.Add);
    }
}