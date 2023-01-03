using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests.Filesystem;

public class RmCommandTests : FilesystemCommandTestBase
{
    public RmCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<TouchCommand>("test.txt", ref context, out _);
        AssertCommand<RmCommand>("test.txt", ref context);

        AssertEntries(1, ref context);
    }

    [Fact]
    public void InvalidName()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<TouchCommand>("test.txt", ref context, out _);
        RunCommand<RmCommand>("test2.txt", ref context, out var output);
        Assert.Single(output);
        Assert.Equal(OutputLevel.Error, output[0].Level);

        AssertEntries(2, ref context);
    }
}
