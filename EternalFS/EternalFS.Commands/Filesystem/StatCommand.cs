using System;
using EternalFS.Commands.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Commands.Filesystem;

[Command("stat", true)]
[CommandSummary("Displays info about a file.")]
public partial class StatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var subEntryName))
            throw new CommandExecutionException(CommandExecutionState.InsufficientArguments, nameof(StatCommand));

        var entry = context.Accessor.LocateSubEntry(new(context.CurrentDirectory.FatEntryReference, subEntryName));

        string entryString = subEntryName.GetString();
        PrintEntryInfo(ref context, entry, entryString);

        return CommandExecutionResult.Default;

        static void PrintEntryInfo(ref CommandExecutionContext context, EternalFileSystemEntry entry, string entryName)
        {
            context.Writer.AppendLine($"Name: {entryName}");
            context.Writer.Append($"Created at: {new DateTime(entry.CreatedAt)}");

            if (!entry.IsDirectory)
                context.Writer.Append($"\r\nSize: {entry.Size}B");
        }
    }
}
