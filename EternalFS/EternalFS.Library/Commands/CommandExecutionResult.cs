using System.Collections.Generic;

namespace EternalFS.Library.Commands;

public class CommandExecutionResult
{
    public int ExitCode { get; init; }

    public bool ShouldExit { get; init; }

    public Stack<string> CurrentDirectory { get; init; } = new();
}
