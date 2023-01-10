using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents an enum of various errors that may appear on command execution.
/// </summary>
[StringMap]
public enum EternalFileSystemState
{
    Valid,

    [Map(@"Unable to open file ""{0}"".")]
    CantOpenFile,

    [Map(@"Unable to open directory ""{0}"".")]
    CantOpenDirectory,

    [Map(@"Unable to locate subentry ""{0}"".")]
    CantLocateSubEntry,

    [Map(@"The filename ""{0}"" is invalid.")]
    InvalidFilename,

    [Map(@"The directory name ""{0}"" is invalid.")]
    InvalidDirectoryName,

    [Map(@"Unable to copy ""{0}"" into ""{1}"".")]
    CantCopyFile,

    [Map(@"Unable to delete ""{0}"".")]
    CantDeleteFile,

    [Map(@"A subentry with the name ""{0}"" already exists.")]
    SubEntryExists,

    [Map("The file system is out of memory.")]
    OutOfMemory,

    [Map(@"Can't open the file system ""{0}"".")]
    CantOpenDiskFS,

#if DEBUG
    [Map("{0}\n{1}")]
#else
    [Map("{0}")]
#endif
    ExecutionException,

    Other
}
