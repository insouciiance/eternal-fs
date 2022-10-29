using System.IO;

namespace EternalFS.Library.Filesystem;

public class VirtualEternalFileSystem : EternalFileSystem
{
    private readonly byte[] _data;

    public VirtualEternalFileSystem(byte[] data)
    {
        _data = data;
        Init();
    }

    public override Stream GetStream()
    {
        return new MemoryStream(_data);
    }
}
