using System;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Filesystem.Indexing;
using EternalFS.Library.Utils;

namespace EternalFS.Commands.Filesystem;

[Command("indexfs", true)]
[CommandSummary("Creates an index over the filesystem to allow faster entry search.")]
public partial class IndexfsCommand
{
#if DEBUG
    [ByteSpan("-si")]
    private static partial ReadOnlySpan<byte> ShowIndex();
#endif

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        DictionaryEntryIndexer indexer = new();
        EternalFileSystemIndexerAccessor indexerAccessor = new(indexer);
        
        InsertIndexer(context.Accessor);
        context.Accessor.Initialize(context.FileSystem);

        context.Writer.Append("Created an index over the execution context");

#if DEBUG
        if (context.ValueSpan.Contains(ShowIndex()))
            indexer.WriteInternalIndex(context.Writer);
#endif

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
