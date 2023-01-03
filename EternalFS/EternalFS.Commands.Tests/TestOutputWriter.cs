using System;
using System.Collections.Generic;
using System.Linq;
using EternalFS.Commands.IO;

namespace EternalFS.Commands.Tests;

public class TestOutputWriter : IOutputWriter
{
    public IReadOnlyList<(OutputLevel Level, string Message)> Output => _output;

    private readonly List<(OutputLevel, string)> _output = new();

    public void Write(string text, OutputLevel level)
    {
        if (text.EndsWith(Environment.NewLine))
            text = text[..^Environment.NewLine.Length];

        _output.Add((level, text));
    }

    public IReadOnlyList<string> GetMessages(OutputLevel level)
    {
        return Output
            .Where(t => t.Level == level)
            .Select(t => t.Message)
            .ToList();
    }

    public void Flush() { }
}
