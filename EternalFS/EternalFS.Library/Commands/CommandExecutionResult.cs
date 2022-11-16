namespace EternalFS.Library.Commands;

public class CommandExecutionResult
{
    public static readonly CommandExecutionResult Default = new();

    public bool ShouldExit { get; init; }
}
