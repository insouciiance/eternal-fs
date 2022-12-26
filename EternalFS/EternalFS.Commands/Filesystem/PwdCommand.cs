using EternalFS.Commands.Extensions;

namespace EternalFS.Commands.Filesystem;

[Command("pwd", true)]
[CommandSummary("Prints current working directory.")]
public partial class PwdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Info(string.Join("\\", context.CurrentDirectory.Path));
        return CommandExecutionResult.Default;
    }
}
