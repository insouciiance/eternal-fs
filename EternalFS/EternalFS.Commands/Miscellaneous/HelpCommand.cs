using EternalFS.Commands.Extensions;

namespace EternalFS.Commands.Miscellaneous;

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

            context.Writer.Info($"{command}: {info.Documentation.Summary}");
        }

        context.Writer.Info(@"Command ""man"" or flag ""--help"" may provide more info for individual commands.");

        return CommandExecutionResult.Default;
    }
}
