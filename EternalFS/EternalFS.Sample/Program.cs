using System.IO;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Terminal;

TerminalRunner runner = new();

runner.OnStart += (ref CommandExecutionContext context) =>
{
    RunCommand("mkfs -n=TestFS -s=10000000", ref context);
};

runner.Run();

static void RunCommand(string command, ref CommandExecutionContext context)
{
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command));
    CommandManager.ExecuteCommand(stream, ref context);
}
