using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Miscellaneous;

[Command("echo")]
[CommandDoc("Outputs the argument passed to this command to the output stream.")]
public partial class EchoCommand
{
    [ByteSpan(" > ")]
    private static partial ReadOnlySpan<byte> GetDelimiter();

    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> content = context.ValueSpan.SplitIndex(GetDelimiter());
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex(GetDelimiter(), 1);

        if (fileName == ReadOnlySpan<byte>.Empty)
        {
            context.Writer.WriteLine(Encoding.UTF8.GetString(context.ValueSpan));
            return new();
        }

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        EternalFileSystemFatEntry fileEntry = manager.CreateFile(fileName, directoryEntry);

        manager.WriteToFile(content, fileEntry, directoryEntry);

        return new();
    }
}
