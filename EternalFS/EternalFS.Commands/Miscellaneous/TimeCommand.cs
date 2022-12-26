using System;
using System.Globalization;
using EternalFS.Commands.Extensions;

namespace EternalFS.Commands.Miscellaneous;

[Command("time")]
[CommandSummary("Outputs the current date and time.")]
public partial class TimeCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Info(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        return CommandExecutionResult.Default;
    }
}
