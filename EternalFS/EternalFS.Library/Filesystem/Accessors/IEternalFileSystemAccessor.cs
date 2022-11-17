using System;
using System.Collections.Generic;

namespace EternalFS.Library.Filesystem.Accessors;

public interface IEternalFileSystemAccessor
{
    void Initialize(EternalFileSystem fileSystem);

    EternalFileSystemEntry LocateDirectory(ICollection<string> directoryStack);

    EternalFileSystemEntry LocateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName);

    EternalFileSystemEntry CreateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName, bool isDirectory);

    void DeleteSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName);

    void CopySubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to);

    void WriteFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName, in ReadOnlySpan<byte> content);
}
