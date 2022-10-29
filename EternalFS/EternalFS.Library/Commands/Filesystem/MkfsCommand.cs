using System;
using System.Text;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkfs")]
[CommandDoc("Creates a file system given its name and size.")]
public partial class MkfsCommand
{
    [ByteSpan("-n")]
    private static partial ReadOnlySpan<byte> Name();

    [ByteSpan("-s")]
    private static partial ReadOnlySpan<byte> Size();

    [ByteSpan("-f")]
    private static partial ReadOnlySpan<byte> FileName();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!ArgumentsHelper.TryGetArgumentValue(context.ValueSpan, Name(), out var nameSpan))
            return new() { ExitCode = -1 };

        if (!ArgumentsHelper.TryGetArgumentValue(context.ValueSpan, Size(), out var sizeSpan))
            return new() { ExitCode = -1 };

        EternalFileSystem fs;

        if (!ArgumentsHelper.TryGetArgumentValue(context.ValueSpan, FileName(), out var fileName))
        {
            fs = new VirtualEternalFileSystem(
                Encoding.UTF8.GetString(nameSpan),
                int.Parse(Encoding.UTF8.GetString(sizeSpan)));
        }
        else
        {
            fs = new DiskEternalFileSystem(
                Encoding.UTF8.GetString(fileName),
                Encoding.UTF8.GetString(nameSpan),
                int.Parse(Encoding.UTF8.GetString(sizeSpan)));
        }

        fs.Mount();

        context.CurrentDirectory.Clear();
        context.CurrentDirectory.Add(EternalFileSystemMounter.ROOT_DIRECTORY_NAME);

        context.FileSystem = fs;

        return new();
    }
}
