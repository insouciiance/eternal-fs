using EternalFS.Commands.Filesystem;
using EternalFS.Library.Filesystem.Accessors.Pipeline;

namespace EternalFS.Commands.Tests.Filesystem;

public class IndexfsCommandTests : FilesystemCommandTestBase
{
    public IndexfsCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void CreateIndex()
    {
        CommandExecutionContext context = CreateFileSystemContext();

        Assert.False(HasIndexer(context.Accessor));
        RunCommand<IndexfsCommand>(null, ref context, out _);
        Assert.True(HasIndexer(context.Accessor));
    }
}
