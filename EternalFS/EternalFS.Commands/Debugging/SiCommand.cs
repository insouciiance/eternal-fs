using EternalFS.Commands.Extensions;
using EternalFS.Commands.IO;
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
            WriteInternalIndex(dictIndexer, context.Writer);
            return CommandExecutionResult.Default;
        }

        context.Writer.Info("Unable to locate internal index for the file system.");
        return CommandExecutionResult.Default;
    }

    private static void WriteInternalIndex(DictionaryEntryIndexer indexer, IOutputWriter writer)
    {
        writer.Debug("Internal index map:");

        var index = indexer.GetInternalIndex();

        foreach (var (key, value) in index)
            writer.Debug($$"""{ {{key}}, {{value}} }""");
    }
}
#endif
