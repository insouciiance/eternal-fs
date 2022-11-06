using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cat", true)]
[CommandDoc("Outputs the contents of a given file.")]
public partial class CatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        if (!ValidationHelper.IsFilenameValid(fileName))
        {
            return new()
            {
                State = CommandExecutionState.InvalidFilename,
                MessageArguments = new object?[] { fileName.GetString() }
            };
        }

        EternalFileSystemManager manager = new(context.FileSystem);

        if (!manager.TryOpenDirectory(context.CurrentDirectory, out var directoryEntry))
            return CommandExecutionResult.CantOpenDirectory(context.CurrentDirectory);

        if (!manager.TryOpenFile(fileName, directoryEntry, out var fileEntry))
            return CommandExecutionResult.CantOpenFile(fileName);

        using EternalFileSystemFileStream stream = new(context.FileSystem, fileEntry.FatEntryReference);

        byte[] content = new byte[fileEntry.Size];
        stream.Read(content, 0, content.Length);
        string contentString = Encoding.UTF8.GetString(content);

        context.Writer.Append(contentString);

        return CommandExecutionResult.Default;
    }
}
