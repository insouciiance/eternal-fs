using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace EternalFS.Library.Filesystem.Indexing;

/// <summary>
/// Represents an indexer that caches and locates entries and directories in <see cref="EternalFileSystem"/>.
/// </summary>
public interface IEntryIndexer
{
    void Initialize(EternalFileSystem fileSystem);

    bool TryLocateEntry(in SubEntryInfo info, out EternalFileSystemEntry entry);
    
    bool TryEnumerateEntries(EternalFileSystemFatEntry directory, SearchOption searchOption, [MaybeNullWhen(false)] out IEnumerable<EternalFileSystemEntry> entries);

    void RecordChange(in SubEntryInfo info, EntryChangeKind changeKind);
}
