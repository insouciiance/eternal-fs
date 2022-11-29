using System.IO;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Terminal;

TerminalRunner runner = new();

runner.OnStart += (ref CommandExecutionContext context) =>
{
    RunCommand("mkfs -n=TestFS -s=10000000", ref context);
    FillTree(ref context);

    void FillTree(ref CommandExecutionContext context, int entriesCount = 100, int depth = 5)
    {
        FillTreeInternal(ref context, 0);

        void FillTreeInternal(ref CommandExecutionContext context, int current)
        {
            for (int i = 0; i < entriesCount; i++)
                RunCommand($"echo file n. {i} > file_{i}.txt", ref context);

            if (current < depth)
            {
                RunCommand("mkdir subdir", ref context);
                RunCommand("cd subdir", ref context);
                FillTreeInternal(ref context, current + 1);
            }
        }
    }
};

runner.Run();

void RunCommand(string command, ref CommandExecutionContext context)
{
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command));
    CommandManager.ExecuteCommand(stream, ref context);
}
