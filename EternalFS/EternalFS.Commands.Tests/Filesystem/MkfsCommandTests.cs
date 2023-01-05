using EternalFS.Commands.Filesystem;

namespace EternalFS.Commands.Tests.Filesystem;

public class MkfsCommandTests : FilesystemCommandTestBase
{
    public MkfsCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateContext();

        AssertCommand<MkfsCommand>("-s=1000 -n=TestFS", ref context);
        Assert.Equal(1000, context.FileSystem.Size);
        Assert.Equal("TestFS", context.FileSystem.Name);
    }

    [Fact]
    public void Sequential()
    {
        CommandExecutionContext context = CreateContext();

        AssertCommand<MkfsCommand>("-s=10000 -n=TestFS", ref context);
        RunCommand<TouchCommand>("test.txt", ref context, out _);
        Assert.Equal(10000, context.FileSystem.Size);
        Assert.Equal("TestFS", context.FileSystem.Name);
        AssertEntries(2, ref context);

        AssertCommand<MkfsCommand>("-s=15000 -n=TestFS2", ref context);
        Assert.Equal(15000, context.FileSystem.Size);
        Assert.Equal("TestFS2", context.FileSystem.Name);
        AssertEntries(1, ref context);
    }
}
