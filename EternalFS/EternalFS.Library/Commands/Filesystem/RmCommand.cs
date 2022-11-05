using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rm", true)]
[CommandDoc("Deletes a specified file.")]
public partial class RmCommand
{
    // TODO: handle deletion correctly.
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);

        if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
        {
            return new()
            {
                State = CommandExecutionState.CantOpenDirectory,
                MessageArguments = new[] { string.Join('/', context.CurrentDirectory) }
            };
        }

        manager.DeleteFile(fileName, directoryEntry);

        return new();
    }
}
