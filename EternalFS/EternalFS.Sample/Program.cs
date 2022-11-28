using System.IO;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Terminal;

TerminalRunner runner = new();

runner.OnStart += (ref CommandExecutionContext context) =>
{
    string command = "mkfs -n=TestFS -s=1000000";
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command));
    CommandManager.ExecuteCommand(stream, ref context);
};

runner.Run();
