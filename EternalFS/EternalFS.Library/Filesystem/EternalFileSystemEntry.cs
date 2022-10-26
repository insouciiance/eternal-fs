using System;
using System.Text;
using System.Runtime.InteropServices;

namespace EternalFS.Library.Filesystem;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct EternalFileSystemEntry
{
    public readonly bool IsDirectory;

    public readonly int Size;

    public readonly long CreatedAt;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public readonly byte[] SubEntryName;

    public readonly EternalFileSystemFatEntry FatEntryReference;

    public EternalFileSystemEntry(in ReadOnlySpan<byte> subEntryName, EternalFileSystemFatEntry fatEntryReference)
        : this(true, 0, subEntryName, fatEntryReference) { }

    public EternalFileSystemEntry(int size, in ReadOnlySpan<byte> subEntryName, EternalFileSystemFatEntry fatEntryReference)
        : this(false, size, subEntryName, fatEntryReference) { }

    private EternalFileSystemEntry(bool isDirectory, int size, in ReadOnlySpan<byte> subEntryName, EternalFileSystemFatEntry fatEntryReference)
    {
        IsDirectory = isDirectory;
        Size = size;

        CreatedAt = DateTime.Now.Ticks;

        Span<byte> nameSpan = stackalloc byte[64];
        subEntryName.CopyTo(nameSpan);

        SubEntryName = nameSpan.ToArray();
        FatEntryReference = fatEntryReference;
    }
}
