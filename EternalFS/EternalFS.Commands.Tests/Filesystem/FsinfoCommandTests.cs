using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests.Filesystem;

public class FsinfoCommandTests : FilesystemCommandTestBase
{
    public FsinfoCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        AssertCommand<FsinfoCommand>(null, ref context,
            (OutputLevel.Info, $"Size: {context.FileSystem.Size}B"),
            (OutputLevel.Info, $"Created at: {context.FileSystem.CreatedAt}"),
            (OutputLevel.Info, $"Name: {context.FileSystem.Name}"));
    }

    [Fact]
    public void ChangeFilesystem()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<MkfsCommand>("-s=1000 -n=AnotherFS", ref context, out _);
        AssertCommand<FsinfoCommand>(null, ref context,
            (OutputLevel.Info, "Size: 1000B"),
            (OutputLevel.Info, $"Created at: {context.FileSystem.CreatedAt}"),
            (OutputLevel.Info, "Name: AnotherFS"));
    }
}
