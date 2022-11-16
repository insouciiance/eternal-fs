using System;
using System.Collections.Generic;

namespace EternalFS.Library.Filesystem.Indexing;

public interface IEntryIndexer
{
    void Initialize(EternalFileSystem fileSystem);

    bool TryLocateDirectory(ICollection<string> directoryStack, out EternalFileSystemEntry entry);

    bool TryLocateEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entryName, out EternalFileSystemEntry entry);

    void RecordChange(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entryName, EntryChangeKind changeKind);
}
 