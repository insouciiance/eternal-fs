using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkdir", true)]
[CommandDoc("Creates an empty directory with the given name.")]
public partial class MkdirCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        if (!ValidationHelper.IsDirectoryValid(directoryName))
            throw new EternalFileSystemException(EternalFileSystemState.InvalidDirectoryName, directoryName.GetString());

        var directoryEntry = context.Accessor.LocateDirectory(context.CurrentDirectory);

        context.Accessor.CreateSubEntry(directoryEntry.FatEntryReference, directoryName, true);

        context.Writer.Append($"Created a directory {directoryName.GetString()}");

        return CommandExecutionResult.Default;
    }
}
