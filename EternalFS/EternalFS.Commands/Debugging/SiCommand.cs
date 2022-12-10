using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Commands.Debugging;

#if DEBUG
[Command("si", true)]
[CommandSummary("Outputs the current underlying index, if any. This is for debug purposes only.")]
public partial class SiCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (context.Accessor.TryFind(element => element is EternalFileSystemIndexerAccessor, out var indexer) &&
            ((EternalFileSystemIndexerAccessor)indexer).Indexer is DictionaryEntryIndexer dictIndexer)
        {
            dictIndexer.WriteInternalIndex(context.Writer);
            return CommandExecutionResult.Default;
        }

        context.Writer.AppendLine("Unable to locate internal index for the file system.");
        return CommandExecutionResult.Default;
    }
}
#endif
