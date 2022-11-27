using System;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkdir", true)]
[CommandSummary("Creates an empty directory with the given name.")]
public partial class MkdirCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        if (!ValidationHelper.IsDirectoryValid(directoryName))
            throw new EternalFileSystemException(EternalFileSystemState.InvalidDirectoryName, directoryName.GetString());

        context.Accessor.CreateSubEntry(context.CurrentDirectory.FatEntryReference, directoryName, true);

        context.Writer.Append($"Created a directory {directoryName.GetString()}");

        return CommandExecutionResult.Default;
    }
}
