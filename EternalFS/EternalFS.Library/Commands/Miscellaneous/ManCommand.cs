using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Commands.Miscellaneous;

[Command("man")]
[CommandDoc("Searches the documentation for the given command.")]
public partial class ManCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> commandSpan = context.ValueSpan.SplitIndex();

        if (commandSpan == ReadOnlySpan<byte>.Empty)
        {
            context.Writer.Append("Command name expected.");
            return new() { State = CommandExecutionState.Other };
        }

        string command = commandSpan.GetString();

        if (CommandManager.CommandInfos.TryGetValue(command, out var info) && info.Documentation is { } doc)
        {
            context.Writer.Append($"Summary: {doc.Summary}");
            return CommandExecutionResult.Default;
        }

        context.Writer.Append($@"Can't find documentation for ""{command}"".");
        return new() { State = CommandExecutionState.Other };
    }
}
