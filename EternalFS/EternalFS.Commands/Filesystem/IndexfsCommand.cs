using EternalFS.Commands.Extensions;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Filesystem.Indexing;

namespace EternalFS.Commands.Filesystem;

[Command("indexfs", true)]
[CommandSummary("Creates an index over the filesystem to allow faster entry search.")]
public partial class IndexfsCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        DictionaryEntryIndexer indexer = new();
        EternalFileSystemIndexerAccessor indexerAccessor = new(indexer);
        
        InsertIndexer(context.Accessor);
        context.Accessor.Initialize(context.FileSystem);

        context.Writer.Info("Created an index over the execution context");

        return CommandExecutionResult.Default;

        void InsertIndexer(AccessorPipelineElement element)
        {
            while (element.Next is not EternalFileSystemManager)
                element = element.Next ?? throw new EternalFileSystemException("Unable to create an index for file system: EternalFileSystemManager not found.");

            indexerAccessor.SetNext(element.Next);
            element.SetNext(indexerAccessor);
        }
    }
}
