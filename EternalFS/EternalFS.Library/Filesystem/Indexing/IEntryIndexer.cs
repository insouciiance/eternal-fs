﻿using System;

namespace EternalFS.Library.Filesystem.Indexing;

/// <summary>
/// Represents an indexer that caches and locates entries and directories in <see cref="EternalFileSystem"/>.
/// </summary>
public interface IEntryIndexer
{
    void Initialize(EternalFileSystem fileSystem);

    bool TryLocateEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entryName, out EternalFileSystemEntry entry);

    void RecordChange(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entryName, EntryChangeKind changeKind);
}
