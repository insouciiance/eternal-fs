using System;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Commands.Miscellaneous;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

// ReSharper disable once CheckNamespace
public static partial class CommandManager
{
    [ByteSpan("--help")]
    private static partial ReadOnlySpan<byte> Help();

    [ByteSpan(" > ")]
    private static partial ReadOnlySpan<byte> WriteDelimiter();

    static partial void PreprocessCommand(ref CommandExecutionContext context, in ReadOnlySpan<byte> input, ref CommandExecutionResult? result)
    {
        context.ValueSpan = context.ValueSpan.SplitIndex(WriteDelimiter());
        
        int spaceIndex = input.IndexOf(ByteSpanHelper.Space());
        ReadOnlySpan<byte> commandSpan = input[(spaceIndex + 1)..];

        if (context.FileSystem is null &&
            CommandInfos.TryGetValue(Encoding.UTF8.GetString(commandSpan), out var info) &&
            info.NeedsFileSystem)
        {
            context.Writer.Append("This command needs a file system to operate on, no file system was attached.");
            result = new() { ExitCode = -1 };
            return;
        }

        if (!context.ValueSpan.Contains(Help()))
            return;

        context.ValueSpan = commandSpan;
        result = ManCommand.Instance.Execute(ref context);
    }

    static partial void PostProcessCommand(ref CommandExecutionContext context, in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
    {
        ReadOnlySpan<byte> fileName = input.SplitIndex(WriteDelimiter(), 1);

        if (fileName == ReadOnlySpan<byte>.Empty)
            return;

        if (context.FileSystem is null)
        {
            context.Writer.Append("This command needs a file system to operate on, no file system was attached.");
            return;
        }

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        EternalFileSystemFatEntry fileEntry = manager.CreateFile(fileName, directoryEntry).FatEntryReference;

        manager.WriteFile(Encoding.UTF8.GetBytes(context.Writer.ToString()), fileEntry, directoryEntry);
        context.Writer.Clear();
    }
}
