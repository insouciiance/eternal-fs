namespace EternalFS.Library.Commands;

public class CommandExecutionResult
{
    public CommandExecutionState State { get; init; }

    public object?[]? MessageArguments { get; init; }

    public bool ShouldExit { get; init; }
}
