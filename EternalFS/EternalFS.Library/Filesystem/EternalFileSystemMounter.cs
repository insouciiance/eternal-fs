using System;
using System.IO;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Initializers;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Mounts different <see cref="EternalFileSystem"/> instances using <see cref="Mount{T}"/>.
/// </summary>
/// <remarks>
/// Also holds various constant values used on mounting.
/// </remarks>
public static class EternalFileSystemMounter
{
    public const int CLUSTER_SIZE_BYTES = 512;

    public const string ROOT_DIRECTORY_NAME = "ROOT";

    public static readonly EternalFileSystemFatEntry RootDirectoryEntry = new(1);

    public static readonly EternalFileSystemFatEntry FatTerminator = new(0xFF, 0xFF);

    public static readonly EternalFileSystemFatEntry EmptyCluster = new(0x00, 0x00);

    public static void Mount<T>(IEternalFileSystemInitializer<T> initializer)
        where T : EternalFileSystem
    {
        initializer.Allocate();
        using Stream stream = initializer.GetStream();

        WriteHeader(initializer, stream);
        WriteAllocationTable(initializer, stream);
        WriteDataSegment(initializer, stream);
    }

    private static void WriteHeader<T>(IEternalFileSystemInitializer<T> initializer, Stream stream)
        where T : EternalFileSystem
    {
        EternalFileSystemHeader header = new(initializer.Size, initializer.Name, DateTime.Now.Ticks);
        stream.MarshalWriteStructure(header);
    }

    private static void WriteAllocationTable<T>(IEternalFileSystemInitializer<T> initializer, Stream stream)
        where T : EternalFileSystem
    {
        stream.MarshalWriteStructure(FatTerminator);

        stream.MarshalWriteStructure(FatTerminator);
        
        for (int i = 1; i < EternalFileSystemHelper.GetClustersCount(initializer.Size); i++)
            stream.MarshalWriteStructure(EmptyCluster);
    }

    private static void WriteDataSegment<T>(IEternalFileSystemInitializer<T> initializer, Stream stream)
        where T : EternalFileSystem
    {
        for (int i = 0; i < EternalFileSystemHelper.GetClustersCount(initializer.Size); i++)
            stream.Write(new byte[CLUSTER_SIZE_BYTES], 0, CLUSTER_SIZE_BYTES);

        stream.Seek(EternalFileSystemHelper.GetClusterOffset(initializer.Size, RootDirectoryEntry), SeekOrigin.Begin);
        stream.MarshalWriteStructure(1);

        EternalFileSystemEntry self = new(ByteSpanHelper.Period(), RootDirectoryEntry, true);
        stream.MarshalWriteStructure(self);
    }
}
