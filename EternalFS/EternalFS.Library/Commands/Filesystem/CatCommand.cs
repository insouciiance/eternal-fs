using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cat")]
[CommandDoc("Outputs the contents of a given file.")]
public partial class CatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        EternalFileSystemFatEntry fileEntry = manager.OpenFile(fileName, directoryEntry);

        using EternalFileSystemFileStream stream = new(context.FileSystem, fileEntry);

        int contentLength = stream.MarshalReadStructure<int>();

        byte[] content = new byte[contentLength];
        stream.Read(content, 0, content.Length);
        string contentString = Encoding.UTF8.GetString(content);

        context.Writer.WriteLine(contentString);

        return new();
    }
}
