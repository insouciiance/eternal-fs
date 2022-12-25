using EternalFS.Commands.Diagnostics;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Filesystem;

[Command("cp", true)]
[CommandSummary("Copies a file into another.")]
public partial class CpCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var from) ||
            !context.Reader.TryReadPositionalArgument(out var to))
            throw new CommandExecutionException(CommandExecutionState.MissingPositionalArguments, nameof(CpCommand));

        context.Accessor.CopySubEntry(new(context.CurrentDirectory.FatEntryReference, from), new(context.CurrentDirectory.FatEntryReference, to));
        
        context.Writer.Append($"Copied {from.GetString()} to {to.GetString()}");

        return CommandExecutionResult.Default;
    }
}
