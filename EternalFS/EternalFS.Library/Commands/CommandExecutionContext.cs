using System;
using System.Collections.Generic;
using System.IO;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands;

public ref struct CommandExecutionContext
{
    public EternalFileSystem FileSystem { get; internal set; } = null!;
    
    public ReadOnlySpan<byte> ValueSpan { get; internal set; } =  ReadOnlySpan<byte>.Empty;

    public TextWriter Writer { get; init; }

    public List<string> CurrentDirectory { get; init; } = new();

    public CommandExecutionContext(TextWriter writer)
    {
        Writer = writer;
    }
}
