namespace EternalFS.Library.Commands.Filesystem;

[Command("fsinfo", true)]
[CommandDoc("Outputs the information about the filesystem being used.")]
public partial class FsinfoCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        context.Writer.WriteLine($"Size: {context.FileSystem.Size}B");
        context.Writer.WriteLine($"Created at: {context.FileSystem.CreatedAt}");
        context.Writer.WriteLine($"Name: {context.FileSystem.Name}");

        return new();
    }
}
