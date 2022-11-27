namespace EternalFS.Library.Commands.Miscellaneous;

[Command("help")]
[CommandSummary("Outputs a list of available commands with their documentation.")]
public partial class HelpCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        foreach (var (command, info) in CommandManager.CommandInfos)
        {
            if (info.Documentation is null)
                continue;

            context.Writer.AppendLine($"{command}: {info.Documentation.Summary}");
        }

        context.Writer.AppendLine();
        context.Writer.Append(@"Command ""man"" or flag ""--help"" may provide more info for individual commands.");

        return CommandExecutionResult.Default;
    }
}
