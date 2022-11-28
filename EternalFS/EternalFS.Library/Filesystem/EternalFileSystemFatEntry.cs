using System.Runtime.InteropServices;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents a FAT-like entry for a file system entry,
/// used to locate it in an allocation table.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct EternalFileSystemFatEntry
{
    public static readonly int EntrySize = Marshal.SizeOf<EternalFileSystemFatEntry>();

    public readonly byte Byte1;

    public readonly byte Byte2;

    public EternalFileSystemFatEntry(byte b1, byte b2)
    {
        Byte1 = b1;
        Byte2 = b2;
    }

    public EternalFileSystemFatEntry(ushort @ushort)
    {
        Byte1 = (byte)(@ushort & 0xFF00);
        Byte2 = (byte)(@ushort & 0x00FF);
    }

    public static implicit operator ushort(EternalFileSystemFatEntry entry)
    {
        byte byte1 = entry.Byte1;
        byte byte2 = entry.Byte2;

        return (ushort)((byte1 << 8) + byte2);
    }

    public static implicit operator EternalFileSystemFatEntry(ushort entry)
    {
        byte byte1 = (byte)((entry & 0xFF00) >> 8);
        byte byte2 = (byte)(entry & 0x00FF);

        return new(byte1, byte2);
    }
}
