using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests.Filesystem;

public class StatCommandTests : FilesystemCommandTestBase
{
    public StatCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<TouchCommand>("test.txt", ref context, out _);
        RunCommand<StatCommand>("test.txt", ref context, out var output);

        Assert.Equal(3, output.Count);
        Assert.Equal((OutputLevel.Info, "Name: test.txt"), output[0]);
        Assert.Equal(OutputLevel.Info, output[1].Level);
        Assert.StartsWith("Created at: ", output[1].Message);
        Assert.Equal((OutputLevel.Info, "Size: 0B"), output[2]);
    }
}
