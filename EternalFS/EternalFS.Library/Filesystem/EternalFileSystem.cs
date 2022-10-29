using System.IO;

namespace EternalFS.Library.Filesystem;

public abstract class EternalFileSystem
{
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
        return (ushort)((Size - EternalFileSystemHeader.HeaderSize - 2) /
                        (EternalFileSystemFatEntry.EntrySize + EternalFileSystemMounter.CLUSTER_SIZE_BYTES));
    }
}
