using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors.Pipeline;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rmindex", true)]
[CommandSummary("Removes the index from the current execution context. If there is no index, does nothing.")]
public partial class RmindexCommand
{
	public CommandExecutionResult Execute(ref CommandExecutionContext context)
	{
		context.Accessor.Remove(element => element is EternalFileSystemIndexerAccessor);
		context.Writer.Append("Removed the index from the execution context");
		return CommandExecutionResult.Default;
	}
}
