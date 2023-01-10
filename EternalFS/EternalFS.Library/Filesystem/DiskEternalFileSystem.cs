using EternalFS.Library.Diagnostics;
using System.IO;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents an <see cref="EternalFileSystem"/> that stores its contents in a file on a disk.
/// </summary>
public class DiskEternalFileSystem : EternalFileSystem
{
    public const string EXTENSION = "etfs";

    public string FileName { get; }

    public DiskEternalFileSystem(string fileName)
    {
        FileName = fileName;

        if (!FileName.EndsWith($"{EXTENSION}"))
            FileName += $".{EXTENSION}";

        if (!File.Exists(FileName))
            throw new EternalFileSystemException(EternalFileSystemState.CantOpenDiskFS, FileName);

        Init();
    }

    public override Stream GetStream()
    {
        return File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
    }
}
