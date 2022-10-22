using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("stat")]
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
            if (entry.IsDirectory)
            {
                context.Writer.WriteLine($"{entryName} is a directory.");
                return;
            }

            context.Writer.WriteLine($"Name: {entryName}");
            context.Writer.WriteLine($"Size: {entry.Size}B");
        }
    }
}
