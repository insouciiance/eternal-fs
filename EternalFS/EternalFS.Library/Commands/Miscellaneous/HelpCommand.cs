namespace EternalFS.Library.Commands.Miscellaneous;

[Command("help")]
[CommandDoc("Outputs a list of available commands with their documentation.")]
public partial class HelpCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        foreach (var (command, info) in CommandManager.Commands)
        {
            if (info is null)
                continue;

            context.Writer.WriteLine($"{command}: {info.Summary}");
        }

        context.Writer.WriteLine();
        context.Writer.WriteLine(@"Command ""man"" or flag ""--help"" may provide more info for individual commands.");

        return new();
    }
}
