using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cp", true)]
[CommandSummary("Copies a file into another.")]
public partial class CpCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> from = context.ValueSpan.SplitIndex();
        ReadOnlySpan<byte> to = context.ValueSpan.SplitIndex(1);

        context.Accessor.CopySubEntry(context.CurrentDirectory.FatEntryReference, from, to);
        
        context.Writer.Append($"Copied {from.GetString()} to {to.GetString()}");

        return CommandExecutionResult.Default;
    }
}
