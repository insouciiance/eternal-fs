﻿using System;
using System.IO;
using System.Text;
using EternalFS.Commands.IO;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Commands.Terminal;

public class TerminalRunner
{
    private const string WRITER_PATH_OUTPUT_FORMAT = "{0}> ";

    public event TerminalStartEventHandler? OnStart;

    public void Run()
    {
        CommandExecutionResult commandResult;
        
        CommandExecutionContext context = new()
        {
            Accessor = SetupAccessors(),
            Writer = new ConsoleOutputWriter()
        };

        OnStart?.Invoke(ref context);

        do
        {
            context = CommandExecutionContext.From(ref context);

            string directoryString = context.CurrentDirectory is { } dir
                ? string.Join("/", dir.Path)
                : string.Empty;

            Console.Write(WRITER_PATH_OUTPUT_FORMAT, directoryString);

            string commandLine = Console.ReadLine()!;
            using MemoryStream inputStream = new(Encoding.UTF8.GetBytes(commandLine));
            commandResult = CommandManager.ExecuteCommand(inputStream, ref context);
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
