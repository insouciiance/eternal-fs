using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cd")]
[CommandDoc("Changes the current working directory.")]
public partial class CdCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        if (directoryName.SequenceEqual(ByteSpanHelper.ParentDirectory()))
            context.CurrentDirectory.RemoveAt(context.CurrentDirectory.Count - 1);
        else
            context.CurrentDirectory.Add(Encoding.UTF8.GetString(directoryName));
    
        new EternalFileSystemManager(context.FileSystem).OpenDirectory(context.CurrentDirectory);

        return new();
    }
}
