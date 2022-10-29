using System;
using System.Text;
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
            context.Writer.WriteLine("Command name expected.");
            return new() { ExitCode = -1 };
        }

        string command = Encoding.UTF8.GetString(commandSpan);

        if (CommandManager.CommandInfos.TryGetValue(command, out var info) && info.Documentation is { } doc)
        {
            context.Writer.WriteLine($"Summary: {doc.Summary}");
            return new();
        }

        context.Writer.WriteLine($@"Can't find documentation for ""{command}"".");
        return new() { ExitCode = -1 };
    }
}
