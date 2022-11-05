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
            result = new() { State = CommandExecutionState.MissingFileSystem };
            return;
        }

        if (!context.ValueSpan.Contains(Help()))
            return;

        context.ValueSpan = commandSpan;
        result = ManCommand.Instance.Execute(ref context);
    }

    static partial void PostProcessCommand(ref CommandExecutionContext context, in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
    {
        HandleFileDelimiter(ref context, input, ref result);
        HandleInvalidExecutionStatus(ref context, ref result);

        void HandleFileDelimiter(ref CommandExecutionContext context, in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
        {
            ReadOnlySpan<byte> filename = input.SplitIndex(WriteDelimiter(), 1);

            if (filename == ReadOnlySpan<byte>.Empty)
                return;

            if (!ValidationHelper.IsFilenameValid(filename))
            {
                result = new()
                {
                    State = CommandExecutionState.InvalidFilename,
                    MessageArguments = new[] { filename.GetString() }
                };

                context.Writer.Clear();

                return;
            }

            if (context.FileSystem is null)
            {
                result = new() { State = CommandExecutionState.MissingFileSystem };
                return;
            }

            EternalFileSystemManager manager = new(context.FileSystem);

            if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
            {
                result = CommandExecutionResult.CantOpenDirectory(context.CurrentDirectory);
                context.Writer.Clear();
                return;
            }

            EternalFileSystemFatEntry fileEntry = manager.CreateFile(filename, directoryEntry).FatEntryReference;

            manager.WriteFile(Encoding.UTF8.GetBytes(context.Writer.ToString()), fileEntry, directoryEntry);
            context.Writer.Clear();
        }

        void HandleInvalidExecutionStatus(ref CommandExecutionContext context, ref CommandExecutionResult result)
        {
            if (result.State is CommandExecutionState.Valid or CommandExecutionState.Other)
                return;

            string formattedMessage = _executionStateMessages.TryGetValue(result.State, out var message)
                    ? string.Format(message, result.MessageArguments ?? Array.Empty<object?>())
                    : $"The command failed to execute: status {result.State} (code {(int)result.State}).";

            context.Writer.Append(formattedMessage);
        }
    }
}
