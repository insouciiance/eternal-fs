using System;

namespace EternalFS.Library.Commands.Miscellaneous;

[Command("time")]
[CommandDoc("Outputs the current date and time.")]
public partial class TimeCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.WriteLine(DateTime.Now.ToString());
        return new();
    }
}
