using EternalFS.Library.Utils;

namespace EternalFS.Commands;

/// <summary>
/// Represents an enum of various errors that may appear on command execution.
/// </summary>
[StringMap]
public enum CommandExecutionState
{
    Valid,

    [Map(@"This command needs a file system to operate on, no file system was attached.")]
    MissingFileSystem,

    [Map(@"Unable to process ""{0}"": command not found.")]
    CommandNotFound,

    [Map(@"Unable to process ""{0}"": insufficient arguments.")]
    InsufficientArguments,

    [Map(@"Unable to process ""{0}"": not all ({1}) required arguments were provided.")]
    MissingPositionalArguments,

#if DEBUG
    [Map("{0}\n{1}")]
#else
    [Map("{0}")]
#endif
    ExecutionException,

    Other
}
