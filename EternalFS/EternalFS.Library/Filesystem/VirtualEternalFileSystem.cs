using System.IO;

namespace EternalFS.Library.Filesystem;

public class VirtualEternalFileSystem : EternalFileSystem
{
    private readonly byte[] _data;

    private readonly EternalFileSystemMounter _mounter;

    public VirtualEternalFileSystem(string name, long length) : base(name, length)
    {
        _data = new byte[length];
        _mounter = new EternalFileSystemMounter(this);
    }

    public override Stream GetStream()
    {
        return new MemoryStream(_data);
    }

    public override void Mount()
    {
        _mounter.Mount();
    }
}
