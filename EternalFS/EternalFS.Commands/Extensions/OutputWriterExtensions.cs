using System;
using System.Diagnostics;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Extensions;

public static class OutputWriterExtensions
{
    public static void WriteLine(this IOutputWriter writer, string text, OutputLevel level)
    {
        writer.Write(text + Environment.NewLine, level);
    }

    public static void Info(this IOutputWriter writer, string text)
    {
        writer.WriteLine(text, OutputLevel.Info);
    }

    public static void Warning(this IOutputWriter writer, string text)
    {
        writer.WriteLine(text, OutputLevel.Warning);
    }

    public static void Error(this IOutputWriter writer, string text)
    {
        writer.WriteLine(text, OutputLevel.Error);
    }

    [Conditional("DEBUG")]
    public static void Debug(this IOutputWriter writer, string text)
    {
        writer.WriteLine(text, OutputLevel.Debug);
    }
}
