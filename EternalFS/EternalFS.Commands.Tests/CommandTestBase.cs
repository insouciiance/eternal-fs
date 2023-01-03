using System.Collections.Generic;
using EternalFS.Commands.IO;
using EternalFS.Commands.Utils;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Validation;

namespace EternalFS.Commands.Tests;

public abstract class CommandTestBase
{
    protected readonly ITestOutputHelper _out;

    protected CommandTestBase(ITestOutputHelper @out)
    {
        _out = @out;
    }

    protected string GetName<T>()
        where T : ICommand
        => T.Info.Name;

    protected void AssertCommand(string command, ref CommandExecutionContext context, params (OutputLevel, string)[] output)
    {
        RunCommand(command, ref context, out var result);
        Assert.Equal(output, result);
    }

    protected void AssertCommand<T>(string? command, ref CommandExecutionContext context, params (OutputLevel, string)[] output)
        where T : ICommand
    {
        RunCommand<T>(command, ref context, out var result);
        Assert.Equal(output, result);
    }

    protected CommandExecutionResult RunCommand(string command, ref CommandExecutionContext context, out IReadOnlyList<(OutputLevel Level, string Message)> output)
    {
        TestOutputWriter writer = new();

        context.Writer = writer;
        var result = CommandHelper.RunCommand(command, ref context);

        foreach (var (level, message) in writer.Output)
            _out.WriteLine($"{level}: {message}");

        output = writer.Output;
        return result;
    }

    protected CommandExecutionResult RunCommand<T>(string? command, ref CommandExecutionContext context, out IReadOnlyList<(OutputLevel Level, string Message)> output)
        where T : ICommand
    {
        command = command is null ? T.Info.Name : $"{T.Info.Name} {command}";
        return RunCommand(command, ref context, out output);
    }

    protected static CommandExecutionContext CreateContext()
    {
        return new() { Accessor = SetupAccessors() };
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
}
