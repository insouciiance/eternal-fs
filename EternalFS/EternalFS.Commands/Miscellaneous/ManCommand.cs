using System;
using EternalFS.Commands.Diagnostics;
using EternalFS.Commands.Extensions;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Miscellaneous;

[Command("man")]
[CommandSummary("Searches the documentation for the given command.")]
public partial class ManCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var commandSpan))
            throw new CommandExecutionException(CommandExecutionState.InsufficientArguments, nameof(ManCommand));

        if (commandSpan == ReadOnlySpan<byte>.Empty)
        {
            context.Writer.Info("Command name expected.");
            throw new CommandExecutionException(CommandExecutionState.Other);
        }

        string command = commandSpan.GetString();

        if (CommandManager.CommandInfos.TryGetValue(command, out var info) && info.Documentation is { } doc)
        {
            context.Writer.Info($"Summary: {doc.Summary}");

            if (doc.Arguments is { Length: > 0 } args)
            {
                context.Writer.Info("\nArguments:");

                foreach (var arg in args)
                    context.Writer.Info($"{arg.Name}   {arg.Description} {(arg.Required ? "[Required]" : string.Empty)}");
            }

            return CommandExecutionResult.Default;
        }

        throw new CommandExecutionException($@"Can't find documentation for ""{command}"".");
    }
}
