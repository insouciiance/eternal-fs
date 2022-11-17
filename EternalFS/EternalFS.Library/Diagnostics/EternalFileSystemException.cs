using EternalFS.Library.Diagnostics;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents an exception occured during file system access.
/// </summary>
public class EternalFileSystemException : StringMapException<EternalFileSystemStateMap, EternalFileSystemState>
{
    public EternalFileSystemException(EternalFileSystemState state, params object?[] args)
        : base(state, args) { }

    protected override string GetUnmappedMessage(EternalFileSystemState state)
    {
        return $"File system failed with code {(int)state}.";
    }
}
