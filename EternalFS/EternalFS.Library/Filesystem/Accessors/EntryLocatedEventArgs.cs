using System;

namespace EternalFS.Library.Filesystem.Accessors;

public class EntryLocatedEventArgs : EventArgs
{
    public EternalFileSystemEntry LocatedEntry { get; }

    public EntryLocatedEventArgs(EternalFileSystemEntry locatedEntry)
    {
        LocatedEntry = locatedEntry;
    }
}
