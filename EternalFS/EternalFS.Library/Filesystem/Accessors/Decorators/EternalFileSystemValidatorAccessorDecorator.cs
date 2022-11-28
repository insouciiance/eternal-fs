using System.Diagnostics.CodeAnalysis;
using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Library.Filesystem.Accessors.Decorators;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> and uses <see cref="IEternalFileSystemValidator"/>
/// before passing the input to the underlying <see cref="IEternalFileSystemAccessor"/>.
/// </summary>
public class EternalFileSystemValidatorAccessorDecorator : EternalFileSystemAccessorDecorator
{
    private readonly IEternalFileSystemValidator _validator;

    [SetsRequiredMembers]
    public EternalFileSystemValidatorAccessorDecorator(IEternalFileSystemAccessor accessor, IEternalFileSystemValidator validator)
        : base(accessor)
    {
        _validator = validator;
    }
    
    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        if (isDirectory)
            _validator.ValidateDirectoryEntry(info.Name);
        else
            _validator.ValidateFileEntry(info.Name);

        return Accessor.CreateSubEntry(info, isDirectory);
    }

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        var subEntry = Accessor.LocateSubEntry(from);

        if (subEntry.IsDirectory)
            _validator.ValidateDirectoryEntry(to.Name);
        else
            _validator.ValidateFileEntry(to.Name);

        Accessor.CopySubEntry(from, to);
    }
}
