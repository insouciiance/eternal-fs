using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("openfs")]
[CommandSummary("Opens an existing file system given its file name.")]
public partial class OpenfsCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileSpan = context.ValueSpan.SplitIndex();
        string fileName = fileSpan.GetString();

        context.CurrentDirectory.Clear();

        DiskEternalFileSystem fs = new(fileName);
        context.FileSystem = fs;
        context.Accessor.Initialize(fs);

        return CommandExecutionResult.Default;
    }
}
