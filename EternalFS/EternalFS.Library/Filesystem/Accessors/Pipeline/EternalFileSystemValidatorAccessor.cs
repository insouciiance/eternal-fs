using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Library.Filesystem.Accessors.Pipeline;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/> and uses <see cref="IEternalFileSystemValidator"/>
/// before passing the input to the underlying <see cref="IEternalFileSystemAccessor"/>.
/// </summary>
public class EternalFileSystemValidatorAccessor : AccessorPipelineElement
{
    private readonly IEternalFileSystemValidator _validator;

    public EternalFileSystemValidatorAccessor(IEternalFileSystemValidator validator)
    {
        _validator = validator;
    }
    
    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        if (isDirectory)
            _validator.ValidateDirectoryEntry(info.Name);
        else
            _validator.ValidateFileEntry(info.Name);

        return base.CreateSubEntry(info, isDirectory);
    }

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        var subEntry = base.LocateSubEntry(from);

        if (subEntry.IsDirectory)
            _validator.ValidateDirectoryEntry(to.Name);
        else
            _validator.ValidateFileEntry(to.Name);

        base.CopySubEntry(from, to);
    }
}
