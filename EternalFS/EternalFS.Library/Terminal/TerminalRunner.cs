using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Terminal;

public class TerminalRunner
{
    private const string WRITER_PATH_OUTPUT_FORMAT = "{0}> ";

    private readonly List<string> _currentDirectory = new();

    private readonly EternalFileSystem _fileSystem;

    public TerminalRunner(EternalFileSystem fileSystem)
    {
        _currentDirectory.Add(EternalFileSystem.ROOT_DIRECTORY_NAME);
        _fileSystem = fileSystem;
    }

    public void Run()
    {
        CommandExecutionResult commandResult;

        do
        {
            string directoryString = string.Join("/", _currentDirectory);
            Console.Write(WRITER_PATH_OUTPUT_FORMAT, directoryString);

            string commandLine = Console.ReadLine()!;
            using MemoryStream inputStream = new(Encoding.UTF8.GetBytes(commandLine));
            commandResult = CommandManager.ExecuteCommand(inputStream, Console.Out, _fileSystem, _currentDirectory);
        } while (!commandResult.ShouldExit);
    }
}
