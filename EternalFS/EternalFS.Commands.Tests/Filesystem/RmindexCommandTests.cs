using EternalFS.Commands.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalFS.Commands.Tests.Filesystem;

public class RmindexCommandTests : FilesystemCommandTestBase
{
    public RmindexCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void RemoveIndex()
    {
        CommandExecutionContext context = CreateFileSystemContext();
        
        Assert.False(HasIndexer(context.Accessor));
        RunCommand<IndexfsCommand>(null, ref context, out _);
        Assert.True(HasIndexer(context.Accessor));
        RunCommand<RmindexCommand>(null, ref context, out _);
        Assert.False(HasIndexer(context.Accessor));
    }
}
