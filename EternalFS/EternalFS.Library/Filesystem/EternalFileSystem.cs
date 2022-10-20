using System.IO;

namespace EternalFS.Library.Filesystem;

public abstract class EternalFileSystem
{
    public const int CLUSTER_SIZE_BYTES = 4096;

    public const int FAT_ENTRY_SIZE_BYTES = 2;

    public const string ROOT_DIRECTORY_NAME = "ROOT";

    public static readonly EternalFileSystemFatEntry RootDirectoryEntry = new(0);

    public string Name { get; }

    public long Size { get; }

    public ushort ClustersCount { get; }

    protected EternalFileSystem(string name, long size)
    {
        Name = name;
        Size = size;
        ClustersCount = GetClustersCount();
    }

    public abstract Stream GetStream();

    public abstract void Mount();

    private ushort GetClustersCount()
    {
        return (ushort)((Size - EternalFileSystemHeader.HeaderSize - 2) / (FAT_ENTRY_SIZE_BYTES + CLUSTER_SIZE_BYTES));
    }
}
