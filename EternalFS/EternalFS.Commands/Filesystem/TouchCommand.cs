using EternalFS.Commands.Diagnostics;
using EternalFS.Commands.Extensions;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Filesystem;

[Command("touch", true)]
[CommandSummary("Creates an empty file with the given name.")]
public partial class TouchCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var filename))
            throw new CommandExecutionException(CommandExecutionState.InsufficientArguments, nameof(TouchCommand));

        context.Accessor.CreateSubEntry(new(context.CurrentDirectory.FatEntryReference, filename), false);
        context.Writer.Info($"Created a file {filename.GetString()}");

        return CommandExecutionResult.Default;
    }
}
