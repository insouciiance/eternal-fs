using System;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cp", true)]
[CommandDoc("Copies a file into another.")]
public partial class CpCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> from = context.ValueSpan.SplitIndex();
        ReadOnlySpan<byte> to = context.ValueSpan.SplitIndex(1);

        var directoryEntry = context.Accessor.LocateDirectory(context.CurrentDirectory);
        context.Accessor.CopyFile(directoryEntry.FatEntryReference, from, to);
        
        context.Writer.Append($"Copied {from.GetString()} to {to.GetString()}");

        return CommandExecutionResult.Default;
    }
}
