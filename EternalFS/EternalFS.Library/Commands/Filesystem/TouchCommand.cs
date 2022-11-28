using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Commands.Filesystem;

[Command("touch", true)]
[CommandSummary("Creates an empty file with the given name.")]
public partial class TouchCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        context.Accessor.CreateSubEntry(new(context.CurrentDirectory.FatEntryReference, fileName), false);
        context.Writer.Append($"Created a file {fileName.GetString()}");

        return CommandExecutionResult.Default;
    }
}
