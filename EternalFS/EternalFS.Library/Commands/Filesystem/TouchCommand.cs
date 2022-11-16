using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands.Filesystem;

[Command("touch", true)]
[CommandDoc("Creates an empty file with the given name.")]
public partial class TouchCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();
        var directoryEntry = context.Accessor.LocateDirectory(context.CurrentDirectory);

        context.Accessor.CreateSubEntry(directoryEntry.FatEntryReference, fileName, false);
        context.Writer.Append($"Created a file {fileName.GetString()}");

        return CommandExecutionResult.Default;
    }
}
