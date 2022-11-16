﻿using System;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Commands.Miscellaneous;
using EternalFS.Library.Diagnostics;
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

    static partial void PreprocessCommand(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> input, ref CommandExecutionResult? result)
    {
        context.ValueSpan = context.ValueSpan.SplitIndex(WriteDelimiter());

        int spaceIndex = input.IndexOf(ByteSpanHelper.Space());
        ReadOnlySpan<byte> commandSpan = input[(spaceIndex + 1)..];

        if (context.FileSystem is null &&
            CommandInfos.TryGetValue(commandSpan.GetString(), out var info) &&
            info.NeedsFileSystem)
            throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

        if (!context.ValueSpan.Contains(Help()))
            return;

        context.ValueSpan = commandSpan;
        result = ManCommand.Instance.Execute(ref context);
    }

    static partial void PostProcessCommand(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
    {
        HandleFileDelimiter(ref context, input, ref result);

        static void HandleFileDelimiter(ref CommandExecutionContext context, in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
        {
            ReadOnlySpan<byte> filename = input.SplitIndex(WriteDelimiter(), 1);

            if (filename == ReadOnlySpan<byte>.Empty)
                return;

            if (!ValidationHelper.IsFilenameValid(filename))
                throw new EternalFileSystemException(EternalFileSystemState.InvalidFilename, filename.GetString());

            if (context.FileSystem is null)
                throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

            var directoryEntry = context.Accessor.LocateDirectory(context.CurrentDirectory);

            context.Accessor.CreateSubEntry(directoryEntry.FatEntryReference, filename, false);
            context.Accessor.WriteFile(directoryEntry.FatEntryReference, filename, Encoding.UTF8.GetBytes(context.Writer.ToString()));
            
            context.Writer.Clear();
        }
    }
}
