namespace EternalFS.Library.Commands;

public enum CommandExecutionState
{
    Valid,

    [CommandStateMessage(@"Unable to open file ""{0}"".")]
    CantOpenFile,

    [CommandStateMessage(@"Unable to open directory ""{0}"".")]
    CantOpenDirectory,

    [CommandStateMessage(@"The filename ""{0}"" is invalid.")]
    InvalidFilename,

    [CommandStateMessage(@"The directory name ""{0}"" is invalid.")]
    InvalidDirectoryName,

    [CommandStateMessage(@"This command needs a file system to operate on, no file system was attached.")]
    MissingFileSystem,

    [CommandStateMessage(@"Unable to process ""{0}"": command not found.")]
    CommandNotFound,

    [CommandStateMessage(@"Unable to copy ""{0}"" into ""{1}"".")]
    CantCopyFile,

    [CommandStateMessage(@"Unable to delete ""{0}"".")]
    CantDeleteFile,

#if DEBUG
    [CommandStateMessage("{0}\n{1}")]
#else
    [CommandStateMessage("{0}")]
#endif
    ExecutionException,

    Other
}
