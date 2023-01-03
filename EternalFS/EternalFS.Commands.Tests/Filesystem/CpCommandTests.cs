using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;
using EternalFS.Commands.Miscellaneous;

namespace EternalFS.Commands.Tests.Filesystem;

public class CpCommandTests : FilesystemCommandTestBase
{
    public CpCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<EchoCommand>("test >=f.txt", ref context, out _);
        AssertCommand<CpCommand>("f.txt f2.txt", ref context, (OutputLevel.Info, "Copied f.txt to f2.txt"));

        AssertFile("f.txt", ref context, "test");
        AssertFile("f2.txt", ref context, "test");
        AssertEntries(3, ref context);
    }

    [Fact]
    public void DifferentDirectory()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<MkdirCommand>("testdir", ref context, out _);
        RunCommand<CdCommand>("testdir", ref context, out _);
        RunCommand<EchoCommand>("test >=f.txt", ref context, out _);
        AssertCommand<CpCommand>("f.txt ../f2.txt", ref context, (OutputLevel.Info, "Copied f.txt to ../f2.txt"));

        RunCommand<CdCommand>("..", ref context, out _);

        AssertFile("testdir/f.txt", ref context, "test");
        AssertFile("f2.txt", ref context, "test");
        AssertEntries(6, ref context);
    }
}
