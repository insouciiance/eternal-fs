using System;

namespace EternalFS.Library.Filesystem;

public readonly ref struct SubEntryInfo
{
    public readonly EternalFileSystemFatEntry FatEntry;

    public readonly ReadOnlySpan<byte> Name;

    public SubEntryInfo(EternalFileSystemFatEntry fatEntry, ReadOnlySpan<byte> name)
    {
        FatEntry = fatEntry;
        Name = name;
    }
}
