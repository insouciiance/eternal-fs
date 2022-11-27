using System;
using System.IO;
using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Library.Filesystem.Accessors;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> and uses <see cref="IEternalFileSystemValidator"/>
/// before passing the input to the underlying <see cref="IEternalFileSystemAccessor"/>.
/// </summary>
public class EternalFileSystemValidatorAccessor : IEternalFileSystemAccessor
{
    private readonly IEternalFileSystemValidator _validator;

    private readonly IEternalFileSystemAccessor _accessor;

    public EternalFileSystemValidatorAccessor(IEternalFileSystemValidator validator, IEternalFileSystemAccessor accessor)
    {
        _validator = validator;
        _accessor = accessor;
    }

    public void Initialize(EternalFileSystem fileSystem)
    {
        _accessor.Initialize(fileSystem);
    }

    public EternalFileSystemEntry LocateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName)
    {
        return _accessor.LocateSubEntry(directoryEntry, subEntryName);
    }

    public EternalFileSystemEntry CreateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName, bool isDirectory)
    {
        if (isDirectory)
            _validator.ValidateDirectoryEntry(subEntryName);
        else
            _validator.ValidateFileEntry(subEntryName);

        return _accessor.CreateSubEntry(directoryEntry, subEntryName, isDirectory);
    }

    public void DeleteSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName)
    {
        _accessor.DeleteSubEntry(directoryEntry, fileName);
    }

    public void CopySubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to)
    {
        var subEntry = _accessor.LocateSubEntry(directoryEntry, from);

        if (subEntry.IsDirectory)
        {
            _validator.ValidateDirectoryEntry(from);
            _validator.ValidateDirectoryEntry(to);
        }
        else
        {
            _validator.ValidateFileEntry(from);
            _validator.ValidateFileEntry(to);
        }

        _accessor.CopySubEntry(directoryEntry, from, to);
    }

    public void WriteFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName, Stream source)
    {
        _validator.ValidateFileEntry(fileName);
        _accessor.WriteFile(directoryEntry, fileName, source);
    }
}
