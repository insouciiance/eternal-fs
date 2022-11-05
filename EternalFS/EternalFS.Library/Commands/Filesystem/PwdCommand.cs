namespace EternalFS.Library.Commands.Filesystem;

[Command("pwd", true)]
[CommandDoc("Prints current working directory.")]
public partial class PwdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Append(string.Join("\\", context.CurrentDirectory));
        return CommandExecutionResult.Default;
    }
}
