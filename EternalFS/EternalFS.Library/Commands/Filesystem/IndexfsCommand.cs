using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Filesystem.Indexing;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("indexfs", true)]
[CommandDoc("Creates an index over the filesystem to allow faster entry search.")]
public partial class IndexfsCommand
{
    [ByteSpan("-si")]
    private static partial ReadOnlySpan<byte> ShowIndex();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        DictionaryEntryIndexer indexer = new();

        context.Accessor = new EternalFileSystemIndexerAccessor(context.Accessor, indexer);
        context.Accessor.Initialize(context.FileSystem);
        context.CurrentDirectory.SetAccessor(context.Accessor);

        context.Writer.Append("Created an index over the execution context");

        if (context.ValueSpan.Contains(ShowIndex()))
            WriteInternalIndex(ref context, indexer);

        return CommandExecutionResult.Default;
    }

    private static void WriteInternalIndex(ref CommandExecutionContext context, DictionaryEntryIndexer indexer)
    {
        context.Writer.AppendLine("\nInternal index map:");

        var index = indexer.GetInternalIndex();

        foreach (var (key, value) in index)
            context.Writer.AppendLine($$"""{ {{key}}, {{value}} }""");
    }
}
