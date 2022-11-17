using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("stat", true)]
[CommandDoc("Displays info about a file.")]
public partial class StatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> subEntryName = context.ValueSpan.SplitIndex();

        string entryString = subEntryName.GetString();
        
        if (EternalFileSystemHelper.TryLocateSubEntry(context.FileSystem, context.CurrentDirectory.FatEntryReference, subEntryName, out var subEntry))
            PrintEntryInfo(ref context, subEntry, entryString);
        else
            context.Writer.Append($"{entryString} not found.");

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
