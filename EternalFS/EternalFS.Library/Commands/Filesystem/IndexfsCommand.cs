using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Indexing;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

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

        context.Accessor = new EternalFileSystemIndexerAccessor(context.Accessor, indexer);
        context.Accessor.Initialize(context.FileSystem);
        context.CurrentDirectory.SetAccessor(context.Accessor);

        context.Writer.Append("Created an index over the execution context");

#if DEBUG
        if (context.ValueSpan.Contains(ShowIndex()))
            indexer.WriteInternalIndex(ref context);
#endif

        return CommandExecutionResult.Default;
    }
}
