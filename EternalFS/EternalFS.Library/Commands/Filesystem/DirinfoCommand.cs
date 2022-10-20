using System.IO;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("dirinfo")]
[CommandDoc("Outputs information about the current directory.")]
public partial class DirinfoCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        EternalFileSystemFatEntry entry = new EternalFileSystemManager(context.FileSystem).OpenDirectory(context.CurrentDirectory);
        using Stream stream = new EternalFileSystemFileStream(context.FileSystem, entry);

        context.Writer.WriteLine($"Subentries count: {stream.ReadByte()}");

        return new();
    }
}
