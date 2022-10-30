using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem;

public class EternalFileSystemManager
{
    private readonly EternalFileSystem _fileSystem;

    public EternalFileSystemManager(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public EternalFileSystemFatEntry OpenDirectory(List<string> directoryStack)
    {
        EternalFileSystemFileStream currentStream = null!;
        EternalFileSystemFatEntry currentFatEntry = default;

        foreach (string entry in directoryStack)
            TraverseDirectories(entry);

        return currentFatEntry;

        void TraverseDirectories(string directoryName)
        {
            if (directoryName == EternalFileSystemMounter.ROOT_DIRECTORY_NAME)
            {
                currentStream = new EternalFileSystemFileStream(_fileSystem, EternalFileSystemMounter.RootDirectoryEntry);
                currentFatEntry = EternalFileSystemMounter.RootDirectoryEntry;
                return;
            }

            int entriesCount = currentStream.MarshalReadStructure<int>();

            for (int i = 0; i < entriesCount; i++)
            {
                var currentEntry = currentStream.MarshalReadStructure<EternalFileSystemEntry>();

                if (Encoding.ASCII.GetString(currentEntry.SubEntryName).TrimEnd('\0') == directoryName)
                {
                    currentStream.Dispose();
                    currentStream = new EternalFileSystemFileStream(_fileSystem, currentEntry.FatEntryReference);
                    currentFatEntry = currentEntry.FatEntryReference;
                    return;
                }
            }
        }
    }

    public EternalFileSystemEntry OpenFile(in ReadOnlySpan<byte> fileName, EternalFileSystemFatEntry directoryEntry)
    {
        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        int entriesCount = stream.MarshalReadStructure<int>();

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

            if (currentEntry.SubEntryName.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(fileName))
                return currentEntry;
        }

        return default;
    }

    public EternalFileSystemFatEntry CreateDirectory(in ReadOnlySpan<byte> directoryName, EternalFileSystemFatEntry directoryEntry)
    {
        using Stream fsStream = _fileSystem.GetStream();

        if (!EternalFileSystemHelper.TryAllocateNewFatEntry(_fileSystem, out var newEntry))
            throw new OutOfMemoryException();

        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        int entriesCount = OverwriteEntriesCount(stream);

        for (int i = 0; i < entriesCount; i++)
            stream.MarshalReadStructure<EternalFileSystemEntry>();

        EternalFileSystemEntry entry = new(directoryName, newEntry);
        stream.MarshalWriteStructure(entry);

        using EternalFileSystemFileStream newDirectoryStream = new(_fileSystem, newEntry);

        newDirectoryStream.MarshalWriteStructure(2);
        newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.Period(), newEntry));
        newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.ParentDirectory(), directoryEntry));

        return newEntry;
    }

    public EternalFileSystemEntry CreateFile(in ReadOnlySpan<byte> fileName, EternalFileSystemFatEntry directoryEntry)
    {
        using Stream fsStream = _fileSystem.GetStream();

        fsStream.Seek(EternalFileSystemHeader.HeaderSize, SeekOrigin.Begin);

        if (!EternalFileSystemHelper.TryAllocateNewFatEntry(_fileSystem, out var newEntry))
            throw new OutOfMemoryException();

        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        int entriesCount = stream.MarshalReadStructure<int>();

        for (int i = 0; i < entriesCount; i++)
            stream.MarshalReadStructure<EternalFileSystemEntry>();

        EternalFileSystemEntry entry = new(0, fileName, newEntry);
        stream.MarshalWriteStructure(entry);

        OverwriteEntriesCount(stream);

        return entry;
    }

    public void DeleteFile(in ReadOnlySpan<byte> fileName, EternalFileSystemFatEntry directoryEntry)
    {
        DeleteFatEntryChain(fileName);
        DeleteDirectoryEntry(fileName);

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

                break;
            }
        }

        void DeleteFatEntryChain(in ReadOnlySpan<byte> fileName)
        {
            EternalFileSystemFatEntry fileEntry = OpenFile(fileName, directoryEntry).FatEntryReference;

            using Stream stream = _fileSystem.GetStream();

            while (fileEntry != EternalFileSystemMounter.FatTerminator)
            {
                int offset = EternalFileSystemHelper.GetFatEntryOffset(fileEntry);

                stream.Seek(offset, SeekOrigin.Begin);
                fileEntry = stream.MarshalReadStructure<EternalFileSystemFatEntry>();

                stream.Seek(offset, SeekOrigin.Begin);
                stream.MarshalWriteStructure(EternalFileSystemMounter.EmptyCluster);
            }
        }
    }

    public void CopyFile(in ReadOnlySpan<byte> from, in ReadOnlySpan<byte> to, EternalFileSystemFatEntry directoryEntry)
    {
        EternalFileSystemEntry fromEntry = OpenFile(from, directoryEntry);
        EternalFileSystemEntry toEntry = CreateFile(to, directoryEntry);

        using EternalFileSystemFileStream fromStream = new(_fileSystem, fromEntry.FatEntryReference);
        using EternalFileSystemFileStream toStream = new(_fileSystem, toEntry.FatEntryReference);

        fromStream.CopyTo(toStream);
        OverwriteFileEntry(toEntry.FatEntryReference, directoryEntry, entry => new(fromEntry.Size, entry.SubEntryName, entry.FatEntryReference));
    }

    public void WriteFile(in ReadOnlySpan<byte> content, EternalFileSystemFatEntry fileEntry, EternalFileSystemFatEntry directoryEntry)
    {
        using (EternalFileSystemFileStream fileStream = new(_fileSystem, fileEntry))
        {
            fileStream.MarshalWriteStructure(content.Length);
            fileStream.Write(content);
        }

        int length = content.Length;
        OverwriteFileEntry(fileEntry, directoryEntry, entry => new(length, entry.SubEntryName, entry.FatEntryReference));
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

    private static int OverwriteEntriesCount(EternalFileSystemFileStream stream, bool increment = true, int delta = 1)
    {
        int amount = increment ? delta : -delta;

        stream.Seek(0);
        int entriesCount = stream.MarshalReadStructure<int>();

        stream.Seek(0);
        stream.MarshalWriteStructure(entriesCount + amount);

        return entriesCount;
    }
}
