namespace EternalFS.Library.Commands;

public interface ICommand
{
    static abstract string Name { get; }

    CommandExecutionResult Execute(ref CommandExecutionContext context);
}
