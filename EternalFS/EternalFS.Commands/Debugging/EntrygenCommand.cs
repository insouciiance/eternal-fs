using System;
using System.IO;
using System.Text;
using EternalFS.Commands.Utils;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Commands.Debugging;

#if DEBUG
[Command("entrygen", true)]
[CommandSummary("Generates dummy file system entries. This is for debug purposes only.")]
[CommandArgument("-d", "Depth of generated directories")]
[CommandArgument("-c", "Number of entries in a directory")]
public partial class EntrygenCommand
{
    private const int DEFAULT_DEPTH = 5;

    private const int DEFAULT_ENTRIES_COUNT = 100;

    [ByteSpan("-d")]
    private partial ReadOnlySpan<byte> Depth();

    [ByteSpan("-c")]
    private partial ReadOnlySpan<byte> EntriesCount();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        int depth = DEFAULT_DEPTH;
        int entriesCount = DEFAULT_ENTRIES_COUNT;

        if (context.Reader.TryReadNamedArgument(Depth(), out var depthArg) && depthArg.HasValue)
            depth = int.Parse(depthArg.Value.GetString());

        if (context.Reader.TryReadNamedArgument(EntriesCount(), out var entriesArg) && entriesArg.HasValue)
            entriesCount = int.Parse(entriesArg.Value.GetString());

        FillTree(ref context);
        CommandHelper.RunCommand("cd /", ref context);

        return CommandExecutionResult.Default;

        void FillTree(ref CommandExecutionContext context)
        {
            FillTreeInternal(ref context, 0);

            void FillTreeInternal(ref CommandExecutionContext context, int current)
            {
                for (int i = 0; i < entriesCount; i++)
                    CommandHelper.RunCommand(@$"echo ""file n. {i}"" >=file_{i}.txt", ref context);

                if (current < depth)
                {
                    CommandHelper.RunCommand("mkdir subdir", ref context);
                    CommandHelper.RunCommand("cd subdir", ref context);
                    FillTreeInternal(ref context, current + 1);
                }
            }
        }
    }
}
#endif
