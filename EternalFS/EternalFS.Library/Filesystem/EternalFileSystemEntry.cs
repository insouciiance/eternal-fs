using System;
using System.Runtime.InteropServices;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents an entry for an <see cref="EternalFileSystem"/>.
/// Holds various information about the entry, e.g., its size, whether it is a directory, etc.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct EternalFileSystemEntry
{
    public static readonly int EntrySize = Marshal.SizeOf<EternalFileSystemEntry>();

    public readonly bool IsDirectory;

    public readonly int Size;

    public readonly long CreatedAt;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public readonly byte[] SubEntryName;

    public readonly EternalFileSystemFatEntry FatEntryReference;

    public EternalFileSystemEntry(in ReadOnlySpan<byte> subEntryName, EternalFileSystemFatEntry fatEntryReference, bool isDirectory = false)
        : this(0, subEntryName, fatEntryReference, isDirectory) { }

    public EternalFileSystemEntry(int size, in ReadOnlySpan<byte> subEntryName, EternalFileSystemFatEntry fatEntryReference)
        : this(size, subEntryName, fatEntryReference, false) { }

    private EternalFileSystemEntry(int size, in ReadOnlySpan<byte> subEntryName, EternalFileSystemFatEntry fatEntryReference, bool isDirectory)
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
