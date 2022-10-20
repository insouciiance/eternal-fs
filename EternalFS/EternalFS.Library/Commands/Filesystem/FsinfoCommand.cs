using System;
using System.IO;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("fsinfo")]
[CommandDoc("Outputs the information about the filesystem being used.")]
public partial class FsinfoCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        using Stream fsStream = context.FileSystem.GetStream();
        EternalFileSystemHeader header = fsStream.MarshalReadStructure<EternalFileSystemHeader>();

        context.Writer.WriteLine($"Size: {header.Size}b");
        context.Writer.WriteLine($"Created at: {new DateTime(header.CreatedAt)}");
        context.Writer.WriteLine($"Name: {Encoding.UTF8.GetString(header.Name).TrimEnd('\0')}");

        return new();
    }
}
