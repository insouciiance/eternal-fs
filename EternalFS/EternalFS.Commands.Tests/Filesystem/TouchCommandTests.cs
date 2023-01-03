using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests.Filesystem;

public class TouchCommandTests : FilesystemCommandTestBase
{
    public TouchCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        AssertCommand<TouchCommand>("test.txt", ref context, (OutputLevel.Info, "Created a file test.txt"));
        AssertFile("test.txt", ref context);
        AssertEntries(2, ref context);
    }

    [Fact]
    public void InvalidName()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<TouchCommand>("test$.txt", ref context, out var output);
        Assert.Single(output);
        Assert.Equal(OutputLevel.Error, output[0].Level);
        AssertEntries(1, ref context);
    }
}
