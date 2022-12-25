using System;
using System.Collections.Generic;
using System.IO;

namespace EternalFS.Library.Filesystem.Accessors;

/// <summary>
/// Handles access to <see cref="EternalFileSystem"/> in a generalized way.
/// </summary>
public interface IEternalFileSystemAccessor
{
    event EventHandler<EntryLocatedEventArgs>? EntryLocated;

    void Initialize(EternalFileSystem fileSystem);

    EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info);

    EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory);

    void DeleteSubEntry(in SubEntryInfo info);

    void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to);

    void WriteFile(in SubEntryInfo info, Stream source, bool append = false);

    IEnumerable<EternalFileSystemEntry> EnumerateEntries(EternalFileSystemFatEntry directory, SearchOption searchOption);

    Stream OpenEntry(EternalFileSystemFatEntry entry);
}
