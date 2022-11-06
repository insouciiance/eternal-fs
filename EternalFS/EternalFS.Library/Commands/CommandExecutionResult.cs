using System;
using System.Collections.Generic;
using System.Text;

namespace EternalFS.Library.Commands;

public class CommandExecutionResult
{
    public static readonly CommandExecutionResult Default = new();

    public CommandExecutionState State { get; init; }

    public object?[]? MessageArguments { get; init; }

    public bool ShouldExit { get; init; }

    public static CommandExecutionResult CantOpenDirectory(IEnumerable<string> stack)
    {
        return new()
        {
            State = CommandExecutionState.CantOpenDirectory,
            MessageArguments = new object?[] { string.Join('/', stack) }
        };
    }

    public static CommandExecutionResult CantOpenFile(in ReadOnlySpan<byte> filename)
    {
        return new()
        {
            State = CommandExecutionState.CantOpenFile,
            MessageArguments = new object?[] { Encoding.UTF8.GetString(filename) }
        };
    }
}
