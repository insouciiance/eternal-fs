namespace EternalFS.Library.Commands;

/// <summary>
/// Represents a general purpose command to execute from CLI.
/// </summary>
public interface ICommand
{
    static abstract CommandInfo Info { get; }

    CommandExecutionResult Execute(ref CommandExecutionContext context);
}
