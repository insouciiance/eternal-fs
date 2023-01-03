using EternalFS.Commands.IO;
using EternalFS.Commands.Miscellaneous;

namespace EternalFS.Commands.Tests.Miscellaneous;

public class EchoCommandTests : CommandTestBase
{
    public EchoCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateContext();
        AssertCommand<EchoCommand>("foo", ref context, (OutputLevel.Info, "foo"));
    }

    [Fact]
    public void WithQuotes()
    {
        CommandExecutionContext context = CreateContext();
        AssertCommand<EchoCommand>(@"""foo baz""", ref context, (OutputLevel.Info, "foo baz"));
    }

    [Fact]
    public void NoQuotes()
    {
        CommandExecutionContext context = CreateContext();
        AssertCommand<EchoCommand>("foo baz", ref context,
            (OutputLevel.Info, "foo"),
            (OutputLevel.Warning, "There were unrecognized parts in the command."));
    }
}
