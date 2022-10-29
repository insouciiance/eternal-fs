using System.IO;

namespace EternalFS.Library.Filesystem;

public class DiskEternalFileSystem : EternalFileSystem
{
    public const string EXTENSION = "efs";

    private readonly string _fileName;

    public DiskEternalFileSystem(string fileName)
    {
        _fileName = fileName;
        Init();
    }

    public override Stream GetStream()
    {
        return File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
    }
}
