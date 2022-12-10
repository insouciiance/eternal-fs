namespace EternalFS.Commands;

/// <summary>
/// 
/// </summary>
public class CommandExecutionResult
{
    public static readonly CommandExecutionResult Default = new();

    public bool ShouldExit { get; init; }
}
