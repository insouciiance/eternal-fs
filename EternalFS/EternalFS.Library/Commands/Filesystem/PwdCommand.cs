using System.Threading.Tasks;

namespace EternalFS.Library.Commands.Filesystem;

[Command("pwd")]
[CommandDoc("Prints current working directory.")]
public partial class PwdCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.WriteLine(string.Join("\\", context.CurrentDirectory));
        return new();
    }
}
