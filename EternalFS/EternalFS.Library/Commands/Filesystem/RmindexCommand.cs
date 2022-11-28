using EternalFS.Library.Filesystem.Accessors.Decorators;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rmindex", true)]
[CommandSummary("Removes the index from the current execution context. If there is no index, does nothing.")]
public partial class RmindexCommand
{
	public CommandExecutionResult Execute(ref CommandExecutionContext context)
	{
		if (context.Accessor is not EternalFileSystemIndexerAccessorDecorator indexerAccessor)
			return CommandExecutionResult.Default;

		context.Accessor = indexerAccessor.Accessor;
		context.Writer.Append("Removed the index from the execution context");
		return CommandExecutionResult.Default;
	}
}
