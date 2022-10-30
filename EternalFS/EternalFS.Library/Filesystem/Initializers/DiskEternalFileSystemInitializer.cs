using System;
using System.IO;

namespace EternalFS.Library.Filesystem.Initializers;

public class DiskEternalFileSystemInitializer : IEternalFileSystemInitializer<DiskEternalFileSystem>
{
    public string Name { get; }

    public long Size { get; }

    public string FileName { get; }

    public DiskEternalFileSystemInitializer(string name, long size, string fileName)
    {
        Name = name;
        Size = size;

        FileName = fileName;
        
        if (!FileName.EndsWith($"{DiskEternalFileSystem.EXTENSION}"))
            FileName += $".{DiskEternalFileSystem.EXTENSION}";
    }

    public void Allocate()
    {
        using Stream stream = File.OpenWrite(FileName);
        ReadOnlySpan<byte> data = new byte[Size];
        stream.Write(data);
    }

    public Stream GetStream()
    {
        return File.OpenWrite(FileName);
    }

    public DiskEternalFileSystem CreateFileSystem() => new(FileName);
}
