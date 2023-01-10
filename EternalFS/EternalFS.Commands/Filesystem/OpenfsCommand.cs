using EternalFS.Commands.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Commands.Filesystem;

[Command("openfs")]
[CommandSummary("Opens an existing file system given its file name.")]
public partial class OpenfsCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!context.Reader.TryReadPositionalArgument(out var fileSpan))
            throw new CommandExecutionException(CommandExecutionState.InsufficientArguments, nameof(OpenfsCommand));

        string fileName = fileSpan.GetString();

        context.CurrentDirectory ??= new();

        context.CurrentDirectory.Clear();

        DiskEternalFileSystem fs = new(fileName);
        context.FileSystem = fs;
        context.Accessor.Initialize(fs);

        return CommandExecutionResult.Default;
    }
}
