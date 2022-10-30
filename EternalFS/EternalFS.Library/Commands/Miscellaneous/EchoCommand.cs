using System.Text;

namespace EternalFS.Library.Commands.Miscellaneous;

[Command("echo")]
[CommandDoc("Outputs the argument passed to this command to the output stream.")]
public partial class EchoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Append(Encoding.UTF8.GetString(context.ValueSpan));
        return new();
    }
}
