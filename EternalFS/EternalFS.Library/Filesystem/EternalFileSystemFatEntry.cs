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

    public readonly byte Byte3;

    public readonly byte Byte4;

    public EternalFileSystemFatEntry(byte b1, byte b2, byte b3, byte b4)
    {
        Byte1 = b1;
        Byte2 = b2;
        Byte3 = b3;
        Byte4 = b4;
    }

    public EternalFileSystemFatEntry(uint @uint)
    {
        Byte1 = (byte)(@uint & 0xFF000000);
        Byte2 = (byte)(@uint & 0x00FF0000);
        Byte2 = (byte)(@uint & 0x0000FF00);
        Byte2 = (byte)(@uint & 0x000000FF);
    }

    public static implicit operator uint(EternalFileSystemFatEntry entry)
    {
        byte byte1 = entry.Byte1;
        byte byte2 = entry.Byte2;
        byte byte3 = entry.Byte3;
        byte byte4 = entry.Byte4;

        return (uint)((byte1 << 24) + (byte2 << 16) + (byte3 << 8) + byte4);
    }

    public static implicit operator EternalFileSystemFatEntry(uint entry)
    {
        byte byte1 = (byte)((entry & 0xFF000000) >> 24);
        byte byte2 = (byte)((entry & 0x00FF0000) >> 16);
        byte byte3 = (byte)((entry & 0x0000FF00) >> 8);
        byte byte4 = (byte)(entry & 0x000000FF);

        return new(byte1, byte2, byte3, byte4);
    }
}
