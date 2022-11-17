using System.IO;

namespace EternalFS.Library.Filesystem.Initializers;

/// <summary>
/// Handles file system creation and initialization.
/// Allows to create different instances of <see cref="EternalFileSystem"/>.
/// </summary>
public interface IEternalFileSystemInitializer<out T>
    where T : EternalFileSystem
{
    long Size { get; }

    string Name { get; }

    void Allocate();

    Stream GetStream();

    T CreateFileSystem();
}
