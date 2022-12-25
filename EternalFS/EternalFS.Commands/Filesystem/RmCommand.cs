using System;
using EternalFS.Commands.Diagnostics;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Filesystem;

[Command("rm", true)]
[CommandSummary("Deletes a specified file.")]
public partial class RmCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var filename))
            throw new CommandExecutionException(CommandExecutionState.InsufficientArguments, nameof(RmCommand));

        context.Accessor.DeleteSubEntry(new(context.CurrentDirectory.FatEntryReference, filename));

        return CommandExecutionResult.Default;
    }
}
