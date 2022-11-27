namespace EternalFS.Library.Commands.Miscellaneous;

[Command("exit")]
[CommandSummary("Exits the terminal.")]
public partial class ExitCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext _)
    {
        return new() { ShouldExit = true };
    }
}
