using System;
using System.Text;
using EternalFS.Commands;
using EternalFS.Commands.Terminal;
using EternalFS.Commands.Utils;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

TerminalRunner runner = new();

runner.OnStart += (ref CommandExecutionContext context) =>
{
    CommandHelper.RunCommand(@"mkfs -n=""TestFS"" -s=1000000", ref context);
};

runner.Run();
