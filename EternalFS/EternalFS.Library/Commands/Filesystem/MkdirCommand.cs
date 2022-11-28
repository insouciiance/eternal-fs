using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkdir", true)]
[CommandSummary("Creates an empty directory with the given name.")]
public partial class MkdirCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        context.Accessor.CreateSubEntry(new(context.CurrentDirectory.FatEntryReference, directoryName), true);

        context.Writer.Append($"Created a directory {directoryName.GetString()}");

        return CommandExecutionResult.Default;
    }
}
