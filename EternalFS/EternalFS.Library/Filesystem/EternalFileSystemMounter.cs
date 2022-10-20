using System;
using System.IO;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem;

public class EternalFileSystemMounter
{
    public static readonly EternalFileSystemFatEntry FatTerminator = new(0xFF, 0xFF);

    public static readonly EternalFileSystemFatEntry EmptyCluster = new(0x00, 0x00);

    private readonly EternalFileSystem _fileSystem;

    public EternalFileSystemMounter(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public void Mount()
    {
        using Stream stream = _fileSystem.GetStream();

        WriteHeader(stream);
        WriteAllocationTable(stream);
        WriteDataSegment(stream);
    }

    private void WriteHeader(Stream stream)
    {
        long length = stream.Length;

        EternalFileSystemHeader header = new(
            length,
            _fileSystem.Name,
            DateTime.Now.Ticks);

        stream.MarshalWriteStructure(header);
    }

    private void WriteAllocationTable(Stream stream)
    {
        stream.MarshalWriteStructure(FatTerminator);

        stream.MarshalWriteStructure(FatTerminator);
        
        for (int i = 1; i < _fileSystem.ClustersCount; i++)
            stream.MarshalWriteStructure(EmptyCluster);
    }

    private void WriteDataSegment(Stream stream)
    {
        for (int i = 0; i < _fileSystem.ClustersCount; i++)
            stream.Write(new byte[EternalFileSystem.CLUSTER_SIZE_BYTES], 0, EternalFileSystem.CLUSTER_SIZE_BYTES);

        using EternalFileSystemFileStream fs = new(_fileSystem, EternalFileSystem.RootDirectoryEntry);
        fs.WriteByte(0);
    }
}
