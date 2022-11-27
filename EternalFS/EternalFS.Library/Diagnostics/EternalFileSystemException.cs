using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Diagnostics;

/// <summary>
/// Represents an exception occurred during file system access.
/// </summary>
public class EternalFileSystemException : StringMapException<EternalFileSystemStateMap, EternalFileSystemState>
{
    private readonly string? _message;

    public EternalFileSystemException(EternalFileSystemState state, params object?[] args)
        : base(state, args) { }

    public EternalFileSystemException(string message)
        : base(EternalFileSystemState.Other)
    {
        _message = message;
    }

    protected override string GetUnmappedMessage(EternalFileSystemState state)
    {
        return _message ?? $"File system failed with code {(int)state}.";
    }
}
