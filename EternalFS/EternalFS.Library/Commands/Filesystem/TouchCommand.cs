using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("touch", true)]
[CommandDoc("Creates an empty file with the given name.")]
public partial class TouchCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);

        if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
        {
            return new()
            {
                State = CommandExecutionState.CantOpenDirectory,
                MessageArguments = new[] { string.Join('/', context.CurrentDirectory) }
            };
        }

        manager.CreateFile(fileName, directoryEntry);

        context.Writer.Append($"Created a file {Encoding.UTF8.GetString(fileName)}");

        return new();
    }
}
