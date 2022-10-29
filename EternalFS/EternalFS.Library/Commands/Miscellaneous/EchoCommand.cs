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

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> content = context.ValueSpan.SplitIndex(GetDelimiter());
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex(GetDelimiter(), 1);

        if (fileName == ReadOnlySpan<byte>.Empty)
        {
            context.Writer.WriteLine(Encoding.UTF8.GetString(context.ValueSpan));
            return new();
        }

        if (context.FileSystem is null)
        {
            context.Writer.WriteLine("This command needs a file system to operate on, no file system was attached.");
            return new() { ExitCode = -1 };
        }

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        EternalFileSystemFatEntry fileEntry = manager.CreateFile(fileName, directoryEntry).FatEntryReference;

        manager.WriteToFile(content, fileEntry, directoryEntry);

        return new();
    }
}
