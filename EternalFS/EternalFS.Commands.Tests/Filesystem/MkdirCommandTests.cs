using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests.Filesystem;

public class MkdirCommandTests : FilesystemCommandTestBase
{
    public MkdirCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        AssertCommand<MkdirCommand>("testdir", ref context, (OutputLevel.Info, "Created a directory testdir"));

        AssertEntries(4, ref context);
    }

    [Fact]
    public void NestedDirectories()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        AssertCommand<MkdirCommand>("testdir", ref context, (OutputLevel.Info, "Created a directory testdir"));
        AssertCommand<MkdirCommand>("testdir/testdir", ref context, (OutputLevel.Info, "Created a directory testdir/testdir"));

        AssertEntries(7, ref context);
    }
}
