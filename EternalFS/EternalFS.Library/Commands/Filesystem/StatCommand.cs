using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("stat", true)]
[CommandSummary("Displays info about a file.")]
public partial class StatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> subEntryName = context.ValueSpan.SplitIndex();

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
