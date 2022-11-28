using System;
using System.Collections.Generic;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents a directory in an <see cref="EternalFileSystem"/>.
/// </summary>
public class EternalFileSystemDirectory
{
	public EternalFileSystemFatEntry FatEntryReference => _fatEntriesStack[^1];

	public IReadOnlyCollection<string> Path => _directoriesStack;

	private readonly List<string> _directoriesStack = new() { EternalFileSystemMounter.ROOT_DIRECTORY_NAME };

	private readonly List<EternalFileSystemFatEntry> _fatEntriesStack = new() { EternalFileSystemMounter.RootDirectoryEntry };

	private IEternalFileSystemAccessor _accessor = null!;

	public void SetAccessor(IEternalFileSystemAccessor accessor)
	{
		_accessor = accessor;
	}

	public void Push(EternalFileSystemEntry subDirectory)
    {
        ReadOnlySpan<byte> subDirectoryName = subDirectory.SubEntryName;
        _directoriesStack.Add(subDirectoryName.GetString());
		_fatEntriesStack.Add(subDirectory.FatEntryReference);
	}

	public void Pop()
	{
		_directoriesStack.RemoveAt(_directoriesStack.Count - 1);
		_fatEntriesStack.RemoveAt(_fatEntriesStack.Count - 1);
	}

	public void Clear()
	{
		_directoriesStack.RemoveRange(1, _directoriesStack.Count - 1);
		_fatEntriesStack.RemoveRange(1, _fatEntriesStack.Count - 1);
	}
}
