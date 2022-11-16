using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands;

[StringMap]
public enum CommandExecutionState
{
    Valid,

    [Map(@"This command needs a file system to operate on, no file system was attached.")]
    MissingFileSystem,

    [Map(@"Unable to process ""{0}"": command not found.")]
    CommandNotFound,

#if DEBUG
    [Map("{0}\n{1}")]
#else
    [Map("{0}")]
#endif
    ExecutionException,

    Other
}
