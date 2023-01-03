using EternalFS.Commands.Filesystem;

namespace EternalFS.Commands.Tests.Filesystem;

public class OpenfsCommandTests : FilesystemCommandTestBase
{
    public OpenfsCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void OpenDiskFilesystem()
    {
        CommandExecutionContext context = CreateContext();
        RunCommand<MkfsCommand>("-s=2000 -n=TestFS1 -f=TestFS1", ref context, out _);
        RunCommand<TouchCommand>("test.txt", ref context, out _);
        AssertEntries(2, ref context);
        RunCommand<MkfsCommand>("-s=3000 -n=TestFS2", ref context, out _);
        AssertEntries(1, ref context);
        AssertCommand<OpenfsCommand>("TestFS1", ref context);
        AssertEntries(2, ref context);
    }
}
