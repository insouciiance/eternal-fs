﻿using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rmindex", true)]
[CommandDoc("Removes the index from the current execution context. If there is no index, does nothing.")]
public partial class RmindexCommand
{
	public CommandExecutionResult Execute(ref CommandExecutionContext context)
	{
		if (context.Accessor is not EternalFileSystemIndexerAccessor indexerAccessor)
			return CommandExecutionResult.Default;

		context.Accessor = indexerAccessor.Accessor;
		context.Writer.Append("Removed the index from the execution context");
		return CommandExecutionResult.Default;
	}
}
