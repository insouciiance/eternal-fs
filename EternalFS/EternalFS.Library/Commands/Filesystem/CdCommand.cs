using System;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cd", true)]
[CommandSummary("Changes the current working directory.")]
public partial class CdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        // just make sure the directory exists.
	    var subDirectory = context.Accessor.LocateSubEntry(context.CurrentDirectory.FatEntryReference, directoryName);

	    if (!subDirectory.IsDirectory)
		    throw new CommandExecutionException($"{directoryName.GetString()} is a file.");

		if (directoryName.SequenceEqual(ByteSpanHelper.ParentDirectory()))
			context.CurrentDirectory.Pop();
        else
            context.CurrentDirectory.Push(directoryName);

        return CommandExecutionResult.Default;
    }
}
