using System;
using System.Collections.Generic;
using System.IO;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands;

public ref struct CommandExecutionContext
{
    public EternalFileSystem FileSystem { get; init; }
    
    public ReadOnlySpan<byte> ValueSpan { get; init; }

    public TextWriter Writer { get; set; }

    public List<string> CurrentDirectory { get; init; }

    public CommandExecutionContext(
        EternalFileSystem fileSystem,
        ReadOnlySpan<byte> valueSpan,
        TextWriter writer,
        List<string> currentDirectory)
    {
        FileSystem = fileSystem;
        ValueSpan = valueSpan;
        Writer = writer;
        CurrentDirectory = currentDirectory;
    }
}
