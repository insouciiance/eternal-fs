using System.IO;
using System.Text;

namespace EternalFS.Commands.Utils;

public static class CommandHelper
{
    public static CommandInfo GetInfo<T>()
        where T : ICommand
        => T.Info;

    public static CommandExecutionResult RunCommand(string command, ref CommandExecutionContext context)
    {
        CommandExecutionContext copy = context;
        context.Dispose();
        context = CommandExecutionContext.From(ref copy);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command));
        return CommandManager.ExecuteCommand(stream, ref context);
    }
}
