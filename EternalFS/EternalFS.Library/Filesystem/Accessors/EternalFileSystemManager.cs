using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Accessors;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/>
/// and provides various helper methods to simplify I/O operations.
/// </summary>
public class EternalFileSystemManager : IEternalFileSystemAccessor
{
    private EternalFileSystem _fileSystem = null!;

    public void Initialize(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public EternalFileSystemEntry LocateDirectory(ICollection<string> directoryStack)
    {
        EternalFileSystemFileStream currentStream = null!;
        EternalFileSystemEntry resultEntry = default;

        foreach (string dirString in directoryStack)
            TraverseDirectories(dirString);

        return resultEntry;

        void TraverseDirectories(string directoryName)
        {
            if (directoryName == EternalFileSystemMounter.ROOT_DIRECTORY_NAME)
            {
                currentStream = new EternalFileSystemFileStream(_fileSystem, EternalFileSystemMounter.RootDirectoryEntry);
                resultEntry = new EternalFileSystemEntry(ByteSpanHelper.ForwardSlash(), EternalFileSystemMounter.RootDirectoryEntry);
                return;
            }

            int entriesCount = currentStream.MarshalReadStructure<int>();

            for (int i = 0; i < entriesCount; i++)
            {
                var currentEntry = currentStream.MarshalReadStructure<EternalFileSystemEntry>();

                if (!currentEntry.IsDirectory)
                    continue;

                if (Encoding.ASCII.GetString(currentEntry.SubEntryName).TrimEnd('\0') == directoryName)
                {
                    currentStream.Dispose();
                    currentStream = new EternalFileSystemFileStream(_fileSystem, currentEntry.FatEntryReference);
                    resultEntry = currentEntry;
                    return;
                }
            }

            throw new EternalFileSystemException(EternalFileSystemState.CantOpenDirectory, directoryName);
        }
    }

    public EternalFileSystemEntry LocateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName)
    {
        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        int entriesCount = stream.MarshalReadStructure<int>();

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

            if (currentEntry.SubEntryName.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(subEntryName))
                return currentEntry;
        }

        throw new EternalFileSystemException(EternalFileSystemState.CantLocateSubEntry, subEntryName.GetString());
    }

    public EternalFileSystemEntry LocateFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> subEntryName)
    {
        var subEntry = LocateSubEntry(directoryEntry, subEntryName);

        if (subEntry.IsDirectory)
            throw new EternalFileSystemException(EternalFileSystemState.CantOpenFile, subEntryName.GetString());

        return subEntry;
    }

    public EternalFileSystemEntry CreateSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entrySpan, bool isDirectory = false)
    {
        using Stream fsStream = _fileSystem.GetStream();

        if (!EternalFileSystemHelper.TryAllocateNewFatEntry(_fileSystem, out var newFatEntry))
            throw new OutOfMemoryException();

        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        int entriesCount = OverwriteEntriesCount(stream);

        for (int i = 0; i < entriesCount; i++)
        {
	        var entry = stream.MarshalReadStructure<EternalFileSystemEntry>();
	        ReadOnlySpan<byte> entryName = entry.SubEntryName;

	        if (entrySpan.SequenceEqual(entryName.TrimEnd(ByteSpanHelper.Null())))
	        {
                // fall back, we can't create this entry.
		        OverwriteEntriesCount(stream, false);
		        throw new EternalFileSystemException(EternalFileSystemState.SubEntryExists, entrySpan.GetString());
	        }
        }

        EternalFileSystemEntry newEntry = new(entrySpan, newFatEntry, isDirectory);
        stream.MarshalWriteStructure(newEntry);

        if (isDirectory)
        {
            using EternalFileSystemFileStream newDirectoryStream = new(_fileSystem, newFatEntry);

            newDirectoryStream.MarshalWriteStructure(2);
            newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.Period(), newFatEntry, true));
            newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.ParentDirectory(), directoryEntry, true));
        }

        return newEntry;
    }

    public void DeleteSubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> entrySpan)
    {
        var entry = LocateFile(directoryEntry, entrySpan);

        // TODO: support directories
        if (entry.IsDirectory)
            throw new NotSupportedException();

        DeleteFatEntryChain(entrySpan);
        DeleteDirectoryEntry(entrySpan);

        void DeleteDirectoryEntry(in ReadOnlySpan<byte> fileName)
        {
            using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

            int entriesCount = OverwriteEntriesCount(stream, false);

            for (int i = 0; i < entriesCount; i++)
            {
                long position = stream.Position;
                var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

                if (!currentEntry.SubEntryName.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(fileName))
                    continue;

                for (int j = i; j < entriesCount; j++)
                {
                    stream.Seek(position + EternalFileSystemEntry.EntrySize);
                    currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

                    stream.Seek(position);
                    stream.MarshalWriteStructure(currentEntry);

                    position += EternalFileSystemEntry.EntrySize;
                }

                return;
            }

            throw new EternalFileSystemException(EternalFileSystemState.CantDeleteFile, fileName.GetString());
        }

        void DeleteFatEntryChain(in ReadOnlySpan<byte> fileName)
        {
            var fileEntry = LocateFile(directoryEntry, fileName);

            EternalFileSystemFatEntry fatRef = fileEntry.FatEntryReference;

            using Stream stream = _fileSystem.GetStream();

            while (fatRef != EternalFileSystemMounter.FatTerminator)
            {
                int offset = EternalFileSystemHelper.GetFatEntryOffset(fatRef);

                stream.Seek(offset, SeekOrigin.Begin);
                fatRef = stream.MarshalReadStructure<EternalFileSystemFatEntry>();

                stream.Seek(offset, SeekOrigin.Begin);
                stream.MarshalWriteStructure(EternalFileSystemMounter.EmptyCluster);
            }
        }
    }

    public void CopySubEntry(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to)
    {
        // TODO: support directories
        var fromEntry = LocateFile(directoryEntry, from);

        EternalFileSystemEntry toEntry = CreateSubEntry(directoryEntry, to, true);

        using EternalFileSystemFileStream fromStream = new(_fileSystem, fromEntry.FatEntryReference);
        using EternalFileSystemFileStream toStream = new(_fileSystem, toEntry.FatEntryReference);

        fromStream.CopyTo(toStream);
        OverwriteFileEntry(toEntry.FatEntryReference, directoryEntry, entry => new(fromEntry.Size, entry.SubEntryName, entry.FatEntryReference));
    }

    public void WriteFile(EternalFileSystemFatEntry directoryEntry, in ReadOnlySpan<byte> fileName, Stream source)
    {
        var fileEntry = LocateFile(directoryEntry, fileName);

        using (EternalFileSystemFileStream fileStream = new(_fileSystem, fileEntry.FatEntryReference))
        {
            source.CopyTo(fileStream);
        }

        int length = (int)source.Length;
        OverwriteFileEntry(fileEntry.FatEntryReference, directoryEntry, entry => new(length, entry.SubEntryName, entry.FatEntryReference));
    }

    private void OverwriteFileEntry(
        EternalFileSystemFatEntry fileEntry,
        EternalFileSystemFatEntry directoryEntry,
        Func<EternalFileSystemEntry, EternalFileSystemEntry> overwriteFunc)
    {
        using EternalFileSystemFileStream readStream = new(_fileSystem, directoryEntry);

        int entriesCount = readStream.MarshalReadStructure<int>();

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = readStream.MarshalReadStructure<EternalFileSystemEntry>();

            if (currentEntry.FatEntryReference == fileEntry)
            {
                readStream.Seek(-EternalFileSystemEntry.EntrySize, SeekOrigin.Current);
                readStream.MarshalWriteStructure(overwriteFunc(currentEntry));
            }
        }
    }

    private static int ReadEntriesCount(EternalFileSystemFileStream stream)
    {
	    stream.Seek(0);
	    return stream.MarshalReadStructure<int>();
    }

    private static int OverwriteEntriesCount(EternalFileSystemFileStream stream, bool increment = true, int delta = 1)
    {
        int amount = increment ? delta : -delta;

        int entriesCount = ReadEntriesCount(stream);

        stream.Seek(0);
        stream.MarshalWriteStructure(entriesCount + amount);

        return entriesCount;
    }
}
