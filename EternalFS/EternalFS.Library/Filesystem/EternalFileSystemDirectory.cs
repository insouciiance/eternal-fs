using System;
using System.Collections.Generic;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents a directory in an <see cref="EternalFileSystem"/>.
/// </summary>
public class EternalFileSystemDirectory
{
	public EternalFileSystemFatEntry FatEntryReference => _entriesStack[^1];

	public IReadOnlyList<string> Path => _directoriesStack;

	private readonly List<string> _directoriesStack = new() { EternalFileSystemMounter.ROOT_DIRECTORY_NAME };

	private readonly List<EternalFileSystemFatEntry> _entriesStack = new() { EternalFileSystemMounter.RootDirectoryEntry };

	public void Push(EternalFileSystemEntry subDirectory)
    {
        ReadOnlySpan<byte> subDirectoryName = subDirectory.SubEntryName;
        _directoriesStack.Add(subDirectoryName.TrimEndNull().GetString());
		_entriesStack.Add(subDirectory.FatEntryReference);
	}

	public void Pop()
	{
		_directoriesStack.RemoveAt(_directoriesStack.Count - 1);
		_entriesStack.RemoveAt(_entriesStack.Count - 1);
	}

	public void Clear()
	{
		_directoriesStack.RemoveRange(1, _directoriesStack.Count - 1);
		_entriesStack.RemoveRange(1, _entriesStack.Count - 1);
	}
}
