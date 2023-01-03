using EternalFS.Commands.Filesystem;
using EternalFS.Commands.IO;
using EternalFS.Library.Filesystem;

namespace EternalFS.Commands.Tests.Filesystem;

public class CdCommandTests : FilesystemCommandTestBase
{
    public CdCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<MkdirCommand>("testdir", ref context, out _);
        AssertCommand<CdCommand>("testdir", ref context);
        Assert.Equal(new[] { EternalFileSystemMounter.ROOT_DIRECTORY_NAME, "testdir" }, context.CurrentDirectory.Path);
    }

    [Fact]
    public void NestedDirectories()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<MkdirCommand>("testdir", ref context, out _);
        RunCommand<MkdirCommand>("testdir/testdir2", ref context, out _);
        RunCommand<MkdirCommand>("testdir/testdir2/testdir3", ref context, out _);
        AssertCommand<CdCommand>("testdir/testdir2/testdir3", ref context);
        Assert.Equal(new[] { EternalFileSystemMounter.ROOT_DIRECTORY_NAME, "testdir", "testdir2", "testdir3" }, context.CurrentDirectory.Path);
    }

    [Fact]
    public void InvalidName()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        RunCommand<MkdirCommand>("testdir", ref context, out _);
        RunCommand<CdCommand>("testdirz", ref context, out var output);
        Assert.Single(output);
        Assert.Equal(OutputLevel.Error, output[0].Level);
    }
}
