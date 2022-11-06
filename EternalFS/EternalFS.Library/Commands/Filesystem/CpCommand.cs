using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cp", true)]
[CommandDoc("Copies a file into another.")]
public partial class CpCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> from = context.ValueSpan.SplitIndex();
        ReadOnlySpan<byte> to = context.ValueSpan.SplitIndex(1);

        EternalFileSystemManager manager = new(context.FileSystem);

        if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
            return CommandExecutionResult.CantOpenDirectory(context.CurrentDirectory);

        if (!manager.TryCopyFile(from, to, directoryEntry))
        {
            return new()
            {
                State = CommandExecutionState.CantCopyFile,
                MessageArguments = new object?[] { from.GetString(), to.GetString() }
            };
        }

        context.Writer.Append($"Copied {from.GetString()} to {to.GetString()}");

        return CommandExecutionResult.Default;
    }
}
