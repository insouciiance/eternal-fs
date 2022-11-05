using System;
using System.Globalization;

namespace EternalFS.Library.Commands.Miscellaneous;

[Command("time")]
[CommandDoc("Outputs the current date and time.")]
public partial class TimeCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        return CommandExecutionResult.Default;
    }
}
