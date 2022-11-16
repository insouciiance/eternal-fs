using System;
using EternalFS.Library.Commands;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rm", true)]
[CommandDoc("Deletes a specified file.")]
public partial class RmCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();
        var directoryEntry = context.Accessor.LocateDirectory(context.CurrentDirectory);
        context.Accessor.DeleteSubEntry(directoryEntry.FatEntryReference, fileName);

        return CommandExecutionResult.Default;
    }
}
