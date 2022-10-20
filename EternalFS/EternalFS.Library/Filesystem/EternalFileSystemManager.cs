using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
            if (directoryName == EternalFileSystem.ROOT_DIRECTORY_NAME)
            {
                currentStream = new EternalFileSystemFileStream(_fileSystem, EternalFileSystem.RootDirectoryEntry);
                return;
            }

            byte entriesCount = (byte)currentStream.ReadByte();

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

    public EternalFileSystemFatEntry OpenFile(in ReadOnlySpan<byte> fileName, EternalFileSystemFatEntry directoryEntry)
    {
        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        byte entriesCount = (byte)stream.ReadByte();

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();

            if (currentEntry.SubEntryName.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(fileName))
                return currentEntry.FatEntryReference;
        }

        return default;
    }

    public EternalFileSystemFatEntry CreateFile(in ReadOnlySpan<byte> fileName, EternalFileSystemFatEntry directoryEntry)
    {
        using Stream fsStream = _fileSystem.GetStream();

        EternalFileSystemFatEntry currentEntry;

        fsStream.Seek(EternalFileSystemHeader.HeaderSize + 4, SeekOrigin.Begin);

        HashSet<EternalFileSystemFatEntry> allocatedEntries = new() { EternalFileSystem.RootDirectoryEntry };

        while ((currentEntry = fsStream.MarshalReadStructure<EternalFileSystemFatEntry>()) != EternalFileSystemMounter.EmptyCluster)
            allocatedEntries.Add(currentEntry);

        for (ushort i = 0; i < 0xFFFF; i++)
        {
            currentEntry = new(i);

            if (!allocatedEntries.Contains(currentEntry))
                break;
        }

        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        byte entriesCount = (byte)stream.ReadByte();

        for (int i = 0; i < entriesCount; i++)
            stream.MarshalReadStructure<EternalFileSystemEntry>();

        EternalFileSystemEntry entry = new(0, fileName, currentEntry);
        stream.MarshalWriteStructure(entry);

        OverwriteEntriesCount(directoryEntry);

        return currentEntry;
    }

    public EternalFileSystemFatEntry CreateDirectory(in ReadOnlySpan<byte> directoryName, EternalFileSystemFatEntry directoryEntry)
    {
        using Stream fsStream = _fileSystem.GetStream();

        if (!EternalFileSystemHelper.TryAllocateNewFatEntry(_fileSystem, out var newEntry))
            throw new OutOfMemoryException();

        using EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);

        byte entriesCount = (byte)stream.ReadByte();

        for (int i = 0; i < entriesCount; i++)
            stream.MarshalReadStructure<EternalFileSystemEntry>();

        EternalFileSystemEntry entry = new(directoryName, newEntry);
        stream.MarshalWriteStructure(entry);

        using EternalFileSystemFileStream newDirectoryStream = new(_fileSystem, newEntry);

        newDirectoryStream.WriteByte(2);
        newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.Period(), newEntry));
        newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.ParentDirectory(), directoryEntry));

        OverwriteEntriesCount(directoryEntry);

        return newEntry;
    }

    public void WriteToFile(in ReadOnlySpan<byte> content, EternalFileSystemFatEntry fileEntry, EternalFileSystemFatEntry directoryEntry)
    {
        using (EternalFileSystemFileStream fileStream = new(_fileSystem, fileEntry))
        {
            fileStream.WriteByte((byte)content.Length);
            fileStream.Write(content);
        }

        using (EternalFileSystemFileStream readStream = new(_fileSystem, directoryEntry))
        {
            byte entriesCount = (byte)readStream.ReadByte();

            for (int i = 0; i < entriesCount; i++)
            {
                var currentEntry = readStream.MarshalReadStructure<EternalFileSystemEntry>();

                if (currentEntry.FatEntryReference == fileEntry)
                {
                    EternalFileSystemEntry newEntry = new(
                        content.Length,
                        currentEntry.SubEntryName,
                        currentEntry.FatEntryReference);

                    readStream.Seek(-Marshal.SizeOf<EternalFileSystemEntry>(), SeekOrigin.Current);
                    readStream.MarshalWriteStructure(newEntry);
                }
            }
        }
    }

    private void OverwriteEntriesCount(EternalFileSystemFatEntry directoryEntry)
    {
        // TODO: handle seek
        EternalFileSystemFileStream stream = new(_fileSystem, directoryEntry);
        byte entriesCount = (byte)stream.ReadByte();
        stream.Dispose();
        stream = new(_fileSystem, directoryEntry);
        stream.WriteByte((byte)(entriesCount + 1));
        stream.Dispose();
    }
}
