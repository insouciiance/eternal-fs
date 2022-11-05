using System.Collections.Generic;

namespace EternalFS.Library.Commands;

public class CommandExecutionResult
{
    public static readonly CommandExecutionResult Default = new();

    public CommandExecutionState State { get; init; }

    public object?[]? MessageArguments { get; init; }

    public bool ShouldExit { get; init; }

    public static CommandExecutionResult CantOpenDirectory(IEnumerable<string> stack)
    {
        return new()
        {
            State = CommandExecutionState.CantOpenDirectory,
            MessageArguments = new[] { string.Join('/', stack) }
        };
    }
}
