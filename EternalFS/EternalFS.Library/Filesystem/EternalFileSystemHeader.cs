using System.Text;
using System.Runtime.InteropServices;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Holds various metadata for an <see cref="EternalFileSystem"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct EternalFileSystemHeader
{
    public static readonly int HeaderSize = Marshal.SizeOf<EternalFileSystemHeader>();

    public readonly long Size;

    public readonly long CreatedAt;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public readonly byte[] Name;

    public EternalFileSystemHeader(
        long size,
        byte[] name,
        long createdAt)
    {
        Size = size;
        Name = name;
        CreatedAt = createdAt;
    }

    public EternalFileSystemHeader(
        long size,
        string name,
        long createdAt)
    {
        Size = size;
        CreatedAt = createdAt;

        byte[] nameBuffer = new byte[16];
        Encoding.ASCII.GetBytes(name, 0, name.Length, nameBuffer, 0);
        Name = nameBuffer;
    }
}
