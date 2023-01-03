using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests.Filesystem;

public class CatCommandTests : FilesystemCommandTestBase
{
    public CatCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand("""echo "dummy text" >=test.txt""", ref context, out _);
        AssertCommand<CatCommand>("test.txt", ref context, (OutputLevel.Info, "dummy text"));
    }

    [Fact]
    public void InvalidName()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand("""echo "dummy text" >=test.txt""", ref context, out _);
        RunCommand<CatCommand>("test123.txt", ref context, out var output);
        Assert.Single(output);
        Assert.Equal(OutputLevel.Error, output[0].Level);
    }
}
