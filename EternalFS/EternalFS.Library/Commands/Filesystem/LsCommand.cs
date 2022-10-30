using System;
using System.Collections.Generic;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("ls", true)]
[CommandDoc("Lists all entries in current working directory.")]
public partial class LsCommand
{
    [ByteSpan("-l")]
    private static partial ReadOnlySpan<byte> LongList();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        EternalFileSystemFatEntry currentDirectory = new EternalFileSystemManager(context.FileSystem).OpenDirectory(context.CurrentDirectory);
        using EternalFileSystemFileStream stream = new(context.FileSystem, currentDirectory);

        int entriesCount = stream.MarshalReadStructure<int>();

        HashSet<EternalFileSystemEntry> subEntries = new();

        for (int i = 0; i < entriesCount; i++)
        {
            EternalFileSystemEntry subEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();
            subEntries.Add(subEntry);
        }

        DumpInfo(ref context, subEntries);

        return new();
    }

    private static void DumpInfo(ref CommandExecutionContext context, HashSet<EternalFileSystemEntry> subEntries)
    {
        DumpDirectories(ref context);
        DumpFiles(ref context);

        void DumpDirectories(ref CommandExecutionContext context)
        {
            foreach (var subEntry in subEntries)
            {
                if (!subEntry.IsDirectory)
                    continue;

                if (context.Writer.Length > 0)
                    context.Writer.AppendLine();

                string directoryName = Encoding.UTF8.GetString(subEntry.SubEntryName).TrimEnd('\0');
                context.Writer.Append($"{directoryName}/");
            }
        }

        void DumpFiles(ref CommandExecutionContext context)
        {
            foreach (var subEntry in subEntries)
            {
                if (subEntry.IsDirectory)
                    continue;

                if (context.Writer.Length > 0)
                    context.Writer.AppendLine();

                string fileName = Encoding.UTF8.GetString(subEntry.SubEntryName).TrimEnd('\0');
                context.Writer.Append(fileName);
                
                if (context.ValueSpan.Contains(LongList()))
                    context.Writer.Append($" [{subEntry.Size}B]");
            }
        }
    }
}
