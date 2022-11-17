using System;
using System.Text;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands;

/// <summary>
/// Represents a context that holds necessary information to execute commands.
/// </summary>
/// <remarks>
/// Passed to <see cref="ICommand.Execute"/>.
/// </remarks>
public ref struct CommandExecutionContext
{
	public EternalFileSystem FileSystem { get; internal set; } = null!;

	public ReadOnlySpan<byte> ValueSpan { get; internal set; } = ReadOnlySpan<byte>.Empty;

	public IEternalFileSystemAccessor Accessor { get; internal set; } = null!;

	public StringBuilder Writer { get; init; } = new();

	public EternalFileSystemDirectory CurrentDirectory { get; init; } = new();

	public CommandExecutionContext() { }
}
