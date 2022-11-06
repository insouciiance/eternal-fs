using EternalFS.Library.Extensions;

namespace EternalFS.Library.Commands.Miscellaneous;

[Command("echo")]
[CommandDoc("Outputs the argument passed to this command to the output stream.")]
public partial class EchoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Append(context.ValueSpan.GetString());
        return CommandExecutionResult.Default;
    }
}
