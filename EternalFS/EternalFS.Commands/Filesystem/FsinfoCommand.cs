using EternalFS.Commands.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Commands.Filesystem;

[Command("fsinfo", true)]
[CommandSummary("Outputs the information about the filesystem being used.")]
public partial class FsinfoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.Info($"Size: {context.FileSystem.Size}B");
        context.Writer.Info($"Created at: {context.FileSystem.CreatedAt}");
        context.Writer.Info($"Name: {context.FileSystem.Name}");

        if (context.FileSystem is DiskEternalFileSystem diskFs)
            context.Writer.Info($"\r\nThe file system is located at: {diskFs.FileName}");

        return CommandExecutionResult.Default;
    }
}
