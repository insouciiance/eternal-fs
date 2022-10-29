namespace EternalFS.Library.Commands;

public interface ICommand
{
    static abstract CommandInfo Info { get; }

    CommandExecutionResult Execute(ref CommandExecutionContext context);
}
