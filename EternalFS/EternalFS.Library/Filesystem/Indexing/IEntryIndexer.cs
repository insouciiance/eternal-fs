using System;
using EternalFS.Library.Commands;

namespace EternalFS.Library.Filesystem.Indexing;

public interface IEntryIndexer
{
    void Initialize(EternalFileSystem fileSystem);

    bool TryLocateEntry(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> entryName, out EternalFileSystemEntry entry);

    void AddEntry(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> entryName);

    void RemoveEntry(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> entryName);
}
