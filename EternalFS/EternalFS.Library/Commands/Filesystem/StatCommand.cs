using System;
using System.Text;
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

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directory = manager.OpenDirectory(context.CurrentDirectory);

        string entryString = Encoding.UTF8.GetString(subEntryName);
        
        if (EternalFileSystemHelper.TryLocateSubEntry(context.FileSystem, directory, subEntryName, out var subEntry))
            PrintEntryInfo(ref context, subEntry, entryString);
        else
            context.Writer.WriteLine($"{entryString} not found.");
        
        return new();

        static void PrintEntryInfo(ref CommandExecutionContext context, EternalFileSystemEntry entry, string entryName)
        {
            context.Writer.WriteLine($"Name: {entryName}");
            context.Writer.WriteLine($"Created at: {new DateTime(entry.CreatedAt)}");

            if (!entry.IsDirectory)
                context.Writer.WriteLine($"Size: {entry.Size}B");
        }
    }
}
