namespace EternalFS.Library.Commands.Filesystem;

[Command("pwd", true)]
[CommandDoc("Prints current working directory.")]
public partial class PwdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.WriteLine(string.Join("\\", context.CurrentDirectory));
        return new();
    }
}
