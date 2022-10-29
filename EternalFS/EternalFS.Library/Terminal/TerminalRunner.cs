using System;
using System.IO;
using System.Text;
using EternalFS.Library.Commands;

namespace EternalFS.Library.Terminal;

public class TerminalRunner
{
    private const string WRITER_PATH_OUTPUT_FORMAT = "{0}> ";

    public event TerminalStartEventHandler? OnStart;

    public void Run()
    {
        CommandExecutionResult commandResult;
        CommandExecutionContext context = new(Console.Out);

        OnStart?.Invoke(ref context);

        do
        {
            string directoryString = string.Join("/", context.CurrentDirectory);
            Console.Write(WRITER_PATH_OUTPUT_FORMAT, directoryString);

            string commandLine = Console.ReadLine()!;
            using MemoryStream inputStream = new(Encoding.UTF8.GetBytes(commandLine));
            commandResult = CommandManager.ExecuteCommand(inputStream, ref context);
        } while (!commandResult.ShouldExit);
    }

    public delegate void TerminalStartEventHandler(ref CommandExecutionContext context);
}
