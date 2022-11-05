using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkdir", true)]
[CommandDoc("Creates an empty directory with the given name.")]
public partial class MkdirCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        if (!ValidationHelper.IsDirectoryValid(directoryName))
        {
            return new()
            {
                State = CommandExecutionState.InvalidDirectoryName,
                MessageArguments = new[] { directoryName.GetString() }
            };
        }

        EternalFileSystemManager manager = new(context.FileSystem);

        if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
            return CommandExecutionResult.CantOpenDirectory(context.CurrentDirectory);

        manager.CreateDirectory(directoryName, directoryEntry);

        context.Writer.Append($"Created a directory {Encoding.UTF8.GetString(directoryName)}");

        return CommandExecutionResult.Default;
    }
}
