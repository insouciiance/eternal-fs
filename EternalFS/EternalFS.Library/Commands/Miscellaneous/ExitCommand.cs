namespace EternalFS.Library.Commands.Miscellaneous;

[Command("exit")]
[CommandDoc("Exits the terminal.")]
public partial class ExitCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext _)
    {
        return new() { ShouldExit = true };
    }
}
