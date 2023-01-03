using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;
using EternalFS.Commands.Miscellaneous;

namespace EternalFS.Commands.Tests.Filesystem;

public class LsCommandTests : FilesystemCommandTestBase
{
    public LsCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<TouchCommand>("f1.txt", ref context, out _);
        RunCommand<EchoCommand>("hi >=f2.txt", ref context, out _);
        RunCommand<MkdirCommand>("testdir", ref context, out _);

        AssertCommand<LsCommand>(null, ref context,
            (OutputLevel.Info, "./"),
            (OutputLevel.Info, "testdir/"),
            (OutputLevel.Info, "f1.txt"),
            (OutputLevel.Info, "f2.txt"));
    }

    [Fact]
    public void LongList()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<TouchCommand>("f1.txt", ref context, out _);
        RunCommand<EchoCommand>("hi >=f2.txt", ref context, out _);
        RunCommand<MkdirCommand>("testdir", ref context, out _);

        AssertCommand<LsCommand>("-l", ref context,
            (OutputLevel.Info, "./"),
            (OutputLevel.Info, "testdir/"),
            (OutputLevel.Info, "f1.txt [0B]"),
            (OutputLevel.Info, "f2.txt [2B]"));
    }
}
