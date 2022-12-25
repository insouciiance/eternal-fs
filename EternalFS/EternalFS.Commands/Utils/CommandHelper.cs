namespace EternalFS.Commands.Utils;

public static class CommandHelper
{
    public static CommandInfo GetInfo<T>()
        where T : ICommand
        => T.Info;
}
