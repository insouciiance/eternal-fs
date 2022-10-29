using System;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("openfs")]
[CommandDoc("Opens an existing file system given its file name.")]
public partial class OpenfsCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileSpan = context.ValueSpan.SplitIndex();
        string fileName = Encoding.UTF8.GetString(fileSpan);

        context.CurrentDirectory.Clear();
        context.CurrentDirectory.Add(EternalFileSystemMounter.ROOT_DIRECTORY_NAME);

        DiskEternalFileSystem fs = new(fileName);
        context.FileSystem = fs;

        return new();
    }
}
