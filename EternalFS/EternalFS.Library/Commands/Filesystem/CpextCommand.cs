using System;
using System.IO;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cpext", true)]
[CommandSummary("Copies the provided file from an external file system into the loaded file system.")]
[CommandArgument(REVERSE_ARG, "Copy the file from loaded file system into the file from an external file system instead.")]
public partial class CpextCommand
{
    private const string REVERSE_ARG = "-r";

    [ByteSpan(REVERSE_ARG)]
    private partial ReadOnlySpan<byte> Reverse();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> from = context.ValueSpan.SplitIndex();
        ReadOnlySpan<byte> to = context.ValueSpan.SplitIndex(1);

        if (context.ValueSpan.Contains(Reverse()))
        {
            // can't use tuple because of ref struct
            ReadOnlySpan<byte> tmp = from;
            from = to;
            to = tmp;
        }

        using FileStream source = File.OpenRead(from.GetString());
        context.Accessor.WriteFile(context.CurrentDirectory.FatEntryReference, to, source);

        return CommandExecutionResult.Default;
    }
}
