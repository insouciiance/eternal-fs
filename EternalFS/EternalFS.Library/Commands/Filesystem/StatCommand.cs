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
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> entryName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);

        using EternalFileSystemFileStream stream = new(context.FileSystem, directoryEntry);

        byte entriesCount = (byte)stream.ReadByte();

        string entryString = Encoding.UTF8.GetString(entryName);

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

            if (currentEntry.SubEntryName.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(entryName))
            {
                PrintEntryInfo(ref context, currentEntry, entryString);
                return new();
            }
        }

        context.Writer.WriteLine($"{entryString} not found.");

        return new();

        static void PrintEntryInfo(ref CommandExecutionContext context, EternalFileSystemEntry entry, string entryName)
        {
            if (entry.IsDirectory)
            {
                context.Writer.Write($"{entryName} is a directory.");
                return;
            }

            context.Writer.WriteLine($"Name: {entryName}");
            context.Writer.WriteLine($"Size: {entry.Size}B");
        }
    }
}
