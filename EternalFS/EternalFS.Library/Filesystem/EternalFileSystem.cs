using System;
using System.IO;
using System.Text;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents an abstract file system.
/// </summary>
public abstract class EternalFileSystem
{
    public string Name { get; private set; } = null!;

    public long Size { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public int ClustersCount => EternalFileSystemHelper.GetClustersCount(Size);

    protected void Init()
    {
        using Stream stream = GetStream();
        var header = stream.MarshalReadStructure<EternalFileSystemHeader>();

        Name = Encoding.UTF8.GetString(header.Name).TrimEnd('\0');
        Size = header.Size;
        CreatedAt = new DateTime(header.CreatedAt);
    }

    public abstract Stream GetStream();
}
