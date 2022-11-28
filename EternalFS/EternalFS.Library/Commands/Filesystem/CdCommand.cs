using System;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cd", true)]
[CommandSummary("Changes the current working directory.")]
public partial class CdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryPath = context.ValueSpan.SplitIndex();

        EternalFileSystemDirectory currentDirectory = context.CurrentDirectory;

        context.Accessor.EntryLocated += LocateHandler;

        try
        {
            var subDirectory = context.Accessor.LocateSubEntry(new(context.CurrentDirectory.FatEntryReference, directoryPath));

            if (!subDirectory.IsDirectory)
                throw new CommandExecutionException($"{directoryPath.GetString()} is a file.");

            return CommandExecutionResult.Default;
        }
        finally
        {
            context.Accessor.EntryLocated -= LocateHandler;
        }

        void LocateHandler(object? _, EntryLocatedEventArgs args) => UpdateDirectory(currentDirectory, args.LocatedEntry);
    }

    private static void UpdateDirectory(EternalFileSystemDirectory directory, EternalFileSystemEntry entry)
    {
        ReadOnlySpan<byte> subEntryName = entry.SubEntryName;

        // just ignore current directory
        if (subEntryName.TrimEndNull().SequenceEqual(ByteSpanHelper.Period()))
            return;

        if (subEntryName.TrimEndNull().SequenceEqual(ByteSpanHelper.ParentDirectory()))
        {
            directory.Pop();
            return;
        }
            
        directory.Push(entry);
    }
}
