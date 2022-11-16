using EternalFS.Library.Commands;

namespace EternalFS.Library.Diagnostics;

public class CommandExecutionException : StringMapException<CommandExecutionStateMap, CommandExecutionState>
{
    public CommandExecutionException(CommandExecutionState state, params object?[] args)
        : base(state, args) { }

    protected override string GetUnmappedMessage(CommandExecutionState state)
    {
        return $"Command failed with code {(int)state}.";
    }
}
