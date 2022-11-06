using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rm", true)]
[CommandDoc("Deletes a specified file.")]
public partial class RmCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);

        if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
            return CommandExecutionResult.CantOpenDirectory(context.CurrentDirectory);

        if (!manager.TryDeleteFile(fileName, directoryEntry))
        {
            return new()
            {
                State = CommandExecutionState.CantDeleteFile,
                MessageArguments = new object?[] { fileName.GetString() }
            };
        }

        return CommandExecutionResult.Default;
    }
}
