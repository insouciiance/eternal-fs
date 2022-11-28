using EternalFS.Library.Filesystem.Accessors.Decorators;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Library.Commands.Filesystem;

#if DEBUG
[Command("si", true)]
[CommandSummary("Outputs the current underlying index, if any. This is for debug purposes only.")]
public partial class SiCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (context.Accessor is EternalFileSystemIndexerAccessorDecorator { Indexer: DictionaryEntryIndexer indexer })
        {
            indexer.WriteInternalIndex(ref context);
            return CommandExecutionResult.Default;
        }

        context.Writer.AppendLine("Unable to locate internal index for the file system.");
        return CommandExecutionResult.Default;
    }
}
#endif
