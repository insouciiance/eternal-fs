using System;
using System.Collections.Generic;
using System.Text;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands;

public ref struct CommandExecutionContext
{
    public EternalFileSystem FileSystem { get; internal set; } = null!;

    public ReadOnlySpan<byte> ValueSpan { get; internal set; } = ReadOnlySpan<byte>.Empty;
    
    public required IEternalFileSystemAccessor Accessor { get; init; }

    public StringBuilder Writer { get; init; } = new();

    public List<string> CurrentDirectory { get; init; } = new();

    public CommandExecutionContext() { }
}
