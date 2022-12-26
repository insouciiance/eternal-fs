using EternalFS.Commands.Diagnostics;
using EternalFS.Commands.Extensions;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Filesystem;

[Command("mkdir", true)]
[CommandSummary("Creates an empty directory with the given name.")]
public partial class MkdirCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var directoryName))
            throw new CommandExecutionException(CommandExecutionState.InsufficientArguments, nameof(MkdirCommand));

        context.Accessor.CreateSubEntry(new(context.CurrentDirectory.FatEntryReference, directoryName), true);

        context.Writer.Info($"Created a directory {directoryName.GetString()}");

        return CommandExecutionResult.Default;
    }
}
