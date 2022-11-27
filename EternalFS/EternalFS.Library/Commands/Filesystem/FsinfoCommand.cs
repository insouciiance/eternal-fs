using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("fsinfo", true)]
[CommandSummary("Outputs the information about the filesystem being used.")]
public partial class FsinfoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.AppendLine($"Size: {context.FileSystem.Size}B");
        context.Writer.AppendLine($"Created at: {context.FileSystem.CreatedAt}");
        context.Writer.Append($"Name: {context.FileSystem.Name}");

        if (context.FileSystem is DiskEternalFileSystem diskFs)
            context.Writer.Append($"\r\nThe file system is located at: {diskFs.FileName}");

        return CommandExecutionResult.Default;
    }
}
