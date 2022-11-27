namespace EternalFS.Library.Commands.Filesystem;

[Command("pwd", true)]
[CommandSummary("Prints current working directory.")]
public partial class PwdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Append(string.Join("\\", context.CurrentDirectory.Path));
        return CommandExecutionResult.Default;
    }
}
