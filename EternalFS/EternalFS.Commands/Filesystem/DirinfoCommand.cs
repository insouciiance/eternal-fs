using System.IO;
using EternalFS.Commands.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Commands.Filesystem;

[Command("dirinfo", true)]
[CommandSummary("Outputs information about the current directory.")]
public partial class DirinfoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        using Stream stream = new EternalFileSystemFileStream(context.FileSystem, context.CurrentDirectory.FatEntryReference);
        context.Writer.Info($"Subentries count: {stream.ReadByte()}");

        return CommandExecutionResult.Default;
    }
}
