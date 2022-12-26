using EternalFS.Commands.Extensions;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Miscellaneous;

[Command("echo")]
[CommandSummary("Outputs the argument passed to this command to the output stream.")]
public partial class EchoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var value))
            value = System.ReadOnlySpan<byte>.Empty;

        context.Writer.Info(value.GetString());
        return CommandExecutionResult.Default;
    }
}
