using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EternalFS.Commands.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Commands.Filesystem;

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

        var subEntries = context.Accessor.EnumerateEntries(context.CurrentDirectory.FatEntryReference, SearchOption.TopDirectoryOnly);

        bool longList = context.Reader.TryReadNamedArgument(LongList(), out _);

        DumpInfo(ref context, subEntries, longList);

        return CommandExecutionResult.Default;
    }

    private static void DumpInfo(ref CommandExecutionContext context, IEnumerable<EternalFileSystemEntry> subEntries, bool longList)
    {
        subEntries = subEntries.ToArray();

        DumpDirectories(ref context);
        DumpFiles(ref context, longList);

        void DumpDirectories(ref CommandExecutionContext context)
        {
            foreach (var subEntry in subEntries)
            {
                if (!subEntry.IsDirectory)
                    continue;

                string directoryName = Encoding.UTF8.GetString(subEntry.SubEntryName).TrimEnd('\0');
                context.Writer.Info($"{directoryName}/");
            }
        }

        void DumpFiles(ref CommandExecutionContext context, bool longList)
        {
            foreach (var subEntry in subEntries)
            {
                if (subEntry.IsDirectory)
                    continue;

                string info = Encoding.UTF8.GetString(subEntry.SubEntryName).TrimEnd('\0');
                
                if (longList)
                    info += $" [{subEntry.Size}B]";
             
                context.Writer.Info(info);
            }
        }
    }
}
