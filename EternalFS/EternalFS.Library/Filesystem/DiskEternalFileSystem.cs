using System.IO;

namespace EternalFS.Library.Filesystem;

public class DiskEternalFileSystem : EternalFileSystem
{
    public const string EXTENSION = "efs";

    public string FileName { get; }

    public DiskEternalFileSystem(string fileName)
    {
        FileName = fileName;

        if (!FileName.EndsWith($"{EXTENSION}"))
            FileName += $".{EXTENSION}";

        Init();
    }

    public override Stream GetStream()
    {
        return File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
    }
}
