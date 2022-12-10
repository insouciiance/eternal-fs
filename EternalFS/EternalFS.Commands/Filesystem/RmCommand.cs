using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Filesystem;

[Command("rm", true)]
[CommandSummary("Deletes a specified file.")]
public partial class RmCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();
        context.Accessor.DeleteSubEntry(new(context.CurrentDirectory.FatEntryReference, fileName));

        return CommandExecutionResult.Default;
    }
}
