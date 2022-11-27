using System.IO;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("dirinfo", true)]
[CommandSummary("Outputs information about the current directory.")]
public partial class DirinfoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        using Stream stream = new EternalFileSystemFileStream(context.FileSystem, context.CurrentDirectory.FatEntryReference);
        context.Writer.Append($"Subentries count: {stream.ReadByte()}");

        return CommandExecutionResult.Default;
    }
}
