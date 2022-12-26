using System;

namespace EternalFS.Commands.IO;

public class ConsoleOutputWriter : IOutputWriter
{
    public void Write(string text, OutputLevel level)
    {
        if (level == OutputLevel.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{IOutputWriter.ERROR_STRING}: ");
        }
        else if (level == OutputLevel.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{IOutputWriter.WARNING_STRING}: ");
        }

        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public void Flush() { }
}
