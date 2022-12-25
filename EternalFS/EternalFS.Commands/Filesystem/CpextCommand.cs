using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using EternalFS.Commands.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Commands.Filesystem;

[Command("cpext", true)]
[CommandSummary("Copies the provided file from an external file system into the loaded file system.")]
[CommandArgument(REVERSE_ARG, "Copy the file from loaded file system into the file from an external file system instead.")]
public partial class CpextCommand
{
    private const string REVERSE_ARG = "-r";

    [ByteSpan(REVERSE_ARG)]
    private static partial ReadOnlySpan<byte> Reverse();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var from) ||
            !context.Reader.TryReadPositionalArgument(out var to))
            throw new CommandExecutionException(CommandExecutionState.MissingPositionalArguments, nameof(CpextCommand), 2);

        if (context.Reader.TryReadNamedArgument(Reverse(), out _))
        {
            CopyReverse(ref context, from, to);
            return CommandExecutionResult.Default;
        }

        using FileStream source = File.Open(from.GetString(), FileMode.OpenOrCreate, FileAccess.Read);
        context.Accessor.WriteFile(new(context.CurrentDirectory.FatEntryReference, to), source);

        return CommandExecutionResult.Default;
    }

    private static void CopyReverse(ref CommandExecutionContext context, scoped ReadOnlySpan<byte> from, scoped ReadOnlySpan<byte> to)
    {
        var fromEntry = context.Accessor.LocateSubEntry(new(context.CurrentDirectory.FatEntryReference, from));
        using EternalFileSystemFileStream source = new(context.FileSystem, fromEntry.FatEntryReference);
        using FileStream destination = File.Open(to.GetString(), FileMode.OpenOrCreate, FileAccess.Write);
        source.CopyTo(destination);
    }
}
