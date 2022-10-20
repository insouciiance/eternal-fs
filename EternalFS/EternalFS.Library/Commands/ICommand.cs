namespace EternalFS.Library.Commands;

public interface ICommand
{
    static abstract string Name { get; }

    static abstract CommandExecutionResult Execute(ref CommandExecutionContext context);
}
