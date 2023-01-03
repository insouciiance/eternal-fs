namespace EternalFS.Commands.Tests.Miscellaneous;

public class ExitCommandTests : CommandTestBase
{
    public ExitCommandTests(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void Simple()
    {
        CommandExecutionContext context = CreateContext();
        var result = RunCommand("exit", ref context, out var output);

        Assert.Empty(output);
        Assert.True(result.ShouldExit);
    }
}
