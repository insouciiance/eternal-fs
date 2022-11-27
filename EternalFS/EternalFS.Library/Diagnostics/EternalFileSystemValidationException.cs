using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Library.Diagnostics;

public class EternalFileSystemValidationException : StringMapException<EternalFileSystemValidationStateMap, EternalFileSystemValidationState>
{
    private readonly string? _message;

    public EternalFileSystemValidationException(EternalFileSystemValidationState state, params object?[] args)
        : base(state, args) { }

    public EternalFileSystemValidationException(string message)
        : base(EternalFileSystemValidationState.Other)
    {
        _message = message;
    }

    protected override string GetUnmappedMessage(EternalFileSystemValidationState state)
    {
        return _message ?? $"Validation failed with code {(int)state}.";
    }
}
