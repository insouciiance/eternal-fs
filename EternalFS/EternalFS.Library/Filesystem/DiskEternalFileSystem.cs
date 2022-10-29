using System.IO;

namespace EternalFS.Library.Filesystem;

public class DiskEternalFileSystem : EternalFileSystem
{
    public const string EXTENSION = "fs";

    private readonly string _fileName;

    private readonly EternalFileSystemMounter _mounter;

    public DiskEternalFileSystem(string fileName, string name, long size) : base(name, size)
    {
        _fileName = $"{fileName}.{EXTENSION}";
        _mounter = new(this);
    }

    public override Stream GetStream()
    {
        return File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    public override void Mount()
    {
        _mounter.Mount();
    }
}
