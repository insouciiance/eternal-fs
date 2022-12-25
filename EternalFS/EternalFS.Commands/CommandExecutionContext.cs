using System.Text;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Utils;

namespace EternalFS.Commands;

/// <summary>
/// Represents a context that holds necessary information to execute commands.
/// </summary>
/// <remarks>
/// Passed to <see cref="ICommand.Execute"/>.
/// </remarks>
public ref struct CommandExecutionContext
{
    public Utf8CommandReader Reader;
	
    public EternalFileSystem FileSystem { get; internal set; } = null!;

	public AccessorPipelineElement Accessor { get; internal set; } = null!;

	public StringBuilder Writer { get; init; } = new();

	public EternalFileSystemDirectory CurrentDirectory { get; init; } = new();

    public ServiceLocator ServiceLocator { get; init; } = new();

	public CommandExecutionContext() { }

	public static CommandExecutionContext From(scoped ref CommandExecutionContext context)
	{
		return new()
		{
			FileSystem = context.FileSystem,
			CurrentDirectory = context.CurrentDirectory,
			Accessor = context.Accessor
        };
	}

    public void Dispose()
    {
		Reader.Dispose();
        Writer.Clear();
    }
}
