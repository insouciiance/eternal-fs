using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("touch")]
[CommandDoc("Creates an empty file with the given name.")]
public partial class TouchCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        manager.CreateFile(fileName, directoryEntry);

        context.Writer.WriteLine($"Created a file {Encoding.UTF8.GetString(fileName)}");

        return new();
    }
}
