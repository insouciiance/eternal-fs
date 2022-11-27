using System;

namespace EternalFS.Library.Filesystem.Validation;

/// <summary>
/// Represents a validator for file/directory entry names.
/// </summary>
public interface IEternalFileSystemValidator
{
    void ValidateFileEntry(in ReadOnlySpan<byte> entry);

    void ValidateDirectoryEntry(in ReadOnlySpan<byte> entry);
}
