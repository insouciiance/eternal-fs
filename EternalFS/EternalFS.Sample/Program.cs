using System;
using System.IO;
using System.Text;
using EternalFS.Commands;
using EternalFS.Commands.Terminal;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

TerminalRunner runner = new();

runner.OnStart += (ref CommandExecutionContext context) =>
{
    RunCommand(@"mkfs -n=""TestFS"" -s=1000000", ref context);
};

runner.Run();

static void RunCommand(string command, ref CommandExecutionContext context)
{
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command));
    CommandManager.ExecuteCommand(stream, ref context);

    if (context.Writer.Length > 0)
    {
        Console.WriteLine(context.Writer);
        context.Writer.Clear();
    }
}
