using System;
using EternalFS.Library.Commands;
using EternalFS.Library.Commands.Miscellaneous;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

internal static partial class CommandManager
{
    [ByteSpan("--help")]
    private static partial ReadOnlySpan<byte> Help();

    static partial void PreprocessCommand(ref CommandExecutionContext context, in ReadOnlySpan<byte> commandSpan, ref CommandExecutionResult? result)
    {
        if (!context.ValueSpan.Contains(Help()))
            return;

        CommandExecutionContext commandContext = new(context.FileSystem, commandSpan, context.Writer, context.CurrentDirectory);
        result = ManCommand.Instance.Execute(ref commandContext);
    }
}
