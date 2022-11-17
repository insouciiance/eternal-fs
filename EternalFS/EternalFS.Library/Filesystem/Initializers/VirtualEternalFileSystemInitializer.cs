using System.IO;

namespace EternalFS.Library.Filesystem.Initializers;

/// <summary>
/// A <see cref="IEternalFileSystemInitializer{T}"/> for <see cref="VirtualEternalFileSystem"/>.
/// </summary>
public class VirtualEternalFileSystemInitializer : IEternalFileSystemInitializer<VirtualEternalFileSystem>
{
    public string Name { get; }

    public long Size { get; }

    private byte[] _data = null!;

    public VirtualEternalFileSystemInitializer(string name, long size)
    {
        Name = name;
        Size = size;
    }

    public void Allocate() => _data = new byte[Size];

    public Stream GetStream() => new MemoryStream(_data);

    public VirtualEternalFileSystem CreateFileSystem() => new(_data);
}
