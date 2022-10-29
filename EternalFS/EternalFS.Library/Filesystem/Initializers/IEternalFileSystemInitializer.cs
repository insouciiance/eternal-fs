using System.IO;

namespace EternalFS.Library.Filesystem.Initializers;

public interface IEternalFileSystemInitializer<out T>
    where T : EternalFileSystem
{
    long Size { get; }

    string Name { get; }

    void Allocate();

    Stream GetStream();

    T CreateFileSystem();
}
