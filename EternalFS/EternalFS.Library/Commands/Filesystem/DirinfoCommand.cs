using System.IO;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("dirinfo", true)]
[CommandDoc("Outputs information about the current directory.")]
public partial class DirinfoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {

        if (!new EternalFileSystemManager(context.FileSystem).TryOpenDirectory(context.CurrentDirectory, out var entry))
            return CommandExecutionResult.CantOpenDirectory(context.CurrentDirectory);

        using Stream stream = new EternalFileSystemFileStream(context.FileSystem, entry);

        context.Writer.Append($"Subentries count: {stream.ReadByte()}");

        return CommandExecutionResult.Default;
    }
}
