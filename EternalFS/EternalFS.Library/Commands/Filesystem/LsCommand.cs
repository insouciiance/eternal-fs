﻿using System;
using System.Collections.Generic;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("ls", true)]
[CommandSummary("Lists all entries in current working directory.")]
[CommandArgument(LONGLIST_ARG, "Display file size as well as file name.")]
public partial class LsCommand
{
    private const string LONGLIST_ARG = "-l";

    [ByteSpan(LONGLIST_ARG)]
    private static partial ReadOnlySpan<byte> LongList();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        using EternalFileSystemFileStream stream = new(context.FileSystem, context.CurrentDirectory.FatEntryReference);

        int entriesCount = stream.MarshalReadStructure<int>();

        HashSet<EternalFileSystemEntry> subEntries = new();

        for (int i = 0; i < entriesCount; i++)
        {
            EternalFileSystemEntry subEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();
            subEntries.Add(subEntry);
        }

        DumpInfo(ref context, subEntries);

        return CommandExecutionResult.Default;
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
