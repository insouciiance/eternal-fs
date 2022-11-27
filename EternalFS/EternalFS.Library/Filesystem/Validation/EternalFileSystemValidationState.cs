using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Validation;

[StringMap]
public enum EternalFileSystemValidationState
{
    [Map("""The entry name may not contain the "{0}" character.""")]
    ForbiddenCharacter,

    [Map("The entry name may not contain leading or trailing spaces.")]
    LeadingOrTrailingSpace,

    [Map("The entry name cannot be empty.")]
    NameEmpty,

    [Map("The file name may not end with a dot.")]
    FileTrailingDot,

    Other
}
