using System;
using System.IO;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Library.Terminal;

public class TerminalRunner
{
    private const string WRITER_PATH_OUTPUT_FORMAT = "{0}> ";

    public event TerminalStartEventHandler? OnStart;

    public void Run()
    {
        CommandExecutionResult commandResult;
        
        CommandExecutionContext context = new()
        {
            Accessor = SetupAccessors()
        };

        OnStart?.Invoke(ref context);

        do
        {
            string directoryString = string.Join("/", context.CurrentDirectory.Path);
            Console.Write(WRITER_PATH_OUTPUT_FORMAT, directoryString);

            string commandLine = Console.ReadLine()!;
            using MemoryStream inputStream = new(Encoding.UTF8.GetBytes(commandLine));
            commandResult = CommandManager.ExecuteCommand(inputStream, ref context);

            if (context.Writer.Length > 0)
                Console.WriteLine(context.Writer);
            
            context.Writer.Clear();
        } while (!commandResult.ShouldExit);
    }

    private static AccessorPipelineElement SetupAccessors()
    {
        EternalFileSystemPathResolverAccessor pathResolver = new();
        EternalFileSystemValidatorAccessor validator = new(new DefaultEternalFileSystemValidator());
        EternalFileSystemManager manager = new();

        pathResolver.SetNext(validator);
        validator.SetNext(manager);

        return pathResolver;
    }

    public delegate void TerminalStartEventHandler(ref CommandExecutionContext context);
}
