using EternalFS.Library.Diagnostics;

namespace EternalFS.Commands.Diagnostics;

/// <summary>
/// Represents an exception occurred during command execution.
/// </summary>
public class CommandExecutionException : StringMapException<CommandExecutionStateMap, CommandExecutionState>
{
    private readonly string? _message;

    public CommandExecutionException(CommandExecutionState state, params object?[] args)
        : base(state, args) { }

    public CommandExecutionException(string message)
	    : base(CommandExecutionState.Other)
    {
	    _message = message;
    }

    protected override string GetUnmappedMessage(CommandExecutionState state)
    {
        return _message ?? $"Command failed with code {(int)state}.";
    }
}
