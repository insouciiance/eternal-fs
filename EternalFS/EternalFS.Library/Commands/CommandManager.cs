using System;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Commands.Miscellaneous;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

// ReSharper disable once CheckNamespace
public static partial class CommandManager
{
    [ByteSpan("--help")]
    private static partial ReadOnlySpan<byte> Help();

    static partial void PreprocessCommand(ref CommandExecutionContext context, in ReadOnlySpan<byte> commandSpan, ref CommandExecutionResult? result)
    {
        if (context.FileSystem is null &&
            CommandInfos.TryGetValue(Encoding.UTF8.GetString(commandSpan), out var info) &&
            info.NeedsFileSystem)
        {
            context.Writer.WriteLine("This command needs a file system to operate on, no file system was attached.");
            result = new() { ExitCode = -1 };
            return;
        }

        if (!context.ValueSpan.Contains(Help()))
            return;

        context.ValueSpan = commandSpan;
        result = ManCommand.Instance.Execute(ref context);
    }
}
