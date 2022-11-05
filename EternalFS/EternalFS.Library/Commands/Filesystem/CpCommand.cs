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

        manager.CopyFile(from, to, directoryEntry);

        context.Writer.Append($"Copied {Encoding.UTF8.GetString(from)} to {Encoding.UTF8.GetString(to)}");

        return CommandExecutionResult.Default;
    }
}
