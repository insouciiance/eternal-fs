﻿using System;
using EternalFS.Commands.Diagnostics;
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
            context.Writer.Append("Command name expected.");
            throw new CommandExecutionException(CommandExecutionState.Other);
        }

        string command = commandSpan.GetString();

        if (CommandManager.CommandInfos.TryGetValue(command, out var info) && info.Documentation is { } doc)
        {
            context.Writer.Append($"Summary: {doc.Summary}");

            if (doc.Arguments is { Length: > 0 } args)
            {
                context.Writer.Append("\nArguments:");

                foreach (var arg in args)
                    context.Writer.Append($"\n{arg.Name}   {arg.Description} {(arg.Required ? "[Required]" : string.Empty)}");
            }

            return CommandExecutionResult.Default;
        }

        throw new CommandExecutionException($@"Can't find documentation for ""{command}"".");
    }
}
