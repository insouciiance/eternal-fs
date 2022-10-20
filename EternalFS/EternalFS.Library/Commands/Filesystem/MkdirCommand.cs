using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkdir")]
[CommandDoc("Creates an empty directory with the given name.")]
public partial class MkdirCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        manager.CreateDirectory(directoryName, directoryEntry);

        context.Writer.WriteLine($"Created a directory {Encoding.UTF8.GetString(directoryName)}");

        return new();
    }
}
