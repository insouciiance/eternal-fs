using System;
using System.IO;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem.Accessors.Pipeline;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Accessors;

/// <summary>
/// Represents an <see cref="IEternalFileSystemAccessor"/>
/// and provides various helper methods to simplify I/O operations.
/// </summary>
public class EternalFileSystemManager : AccessorPipelineElement
{
    private EternalFileSystem _fileSystem = null!;

    public override event EventHandler<EntryLocatedEventArgs>? EntryLocated;

    public override void Initialize(EternalFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public override EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info)
    {
        using EternalFileSystemFileStream stream = new(_fileSystem, info.FatEntry);

        int entriesCount = stream.MarshalReadStructure<int>();

        for (int i = 0; i < entriesCount; i++)
        {
            var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();
            ReadOnlySpan<byte> entryName = currentEntry.SubEntryName;

            if (entryName.TrimEndNull().SequenceEqual(info.Name))
            {
                EntryLocated?.Invoke(this, new(currentEntry));
                return currentEntry;
            }
        }

        throw new EternalFileSystemException(EternalFileSystemState.CantLocateSubEntry, info.Name.GetString());
    }

    public EternalFileSystemEntry LocateFile(in SubEntryInfo info)
    {
        var subEntry = LocateSubEntry(info);

        if (subEntry.IsDirectory)
            throw new EternalFileSystemException(EternalFileSystemState.CantOpenFile, info.Name.GetString());

        return subEntry;
    }

    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        using Stream fsStream = _fileSystem.GetStream();

        if (!EternalFileSystemHelper.TryAllocateNewFatEntry(_fileSystem, out var newFatEntry))
            throw new EternalFileSystemException(EternalFileSystemState.OutOfMemory);

        using EternalFileSystemFileStream stream = new(_fileSystem, info.FatEntry);

        int entriesCount = OverwriteEntriesCount(stream);

        for (int i = 0; i < entriesCount; i++)
        {
            var entry = stream.MarshalReadStructure<EternalFileSystemEntry>();
            ReadOnlySpan<byte> entryName = entry.SubEntryName;

            if (info.Name.SequenceEqual(entryName.TrimEndNull()))
            {
                // fall back, we can't create this entry.
                OverwriteEntriesCount(stream, false);
                throw new EternalFileSystemException(EternalFileSystemState.SubEntryExists, info.Name.GetString());
            }
        }

        EternalFileSystemEntry newEntry = new(info.Name, newFatEntry, isDirectory);
        stream.MarshalWriteStructure(newEntry);

        if (isDirectory)
        {
            using EternalFileSystemFileStream newDirectoryStream = new(_fileSystem, newFatEntry);

            newDirectoryStream.MarshalWriteStructure(2);
            newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.Period(), newFatEntry, true));
            newDirectoryStream.MarshalWriteStructure<EternalFileSystemEntry>(new(ByteSpanHelper.ParentDirectory(), info.FatEntry, true));
        }

        return newEntry;
    }

    public override void DeleteSubEntry(in SubEntryInfo info)
    {
        var entry = LocateFile(info);

        // TODO: support directories
        if (entry.IsDirectory)
            throw new NotSupportedException();

        DeleteFatEntryChain(info);
        DeleteDirectoryEntry(info);

        void DeleteDirectoryEntry(in SubEntryInfo info)
        {
            using EternalFileSystemFileStream stream = new(_fileSystem, info.FatEntry);

            int entriesCount = OverwriteEntriesCount(stream, false);

            for (int i = 0; i < entriesCount; i++)
            {
                long position = stream.Position;
                var currentEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();
                ReadOnlySpan<byte> entryName = currentEntry.SubEntryName;

                if (!entryName.TrimEndNull().SequenceEqual(info.Name))
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

            throw new EternalFileSystemException(EternalFileSystemState.CantDeleteFile, info.Name.GetString());
        }

        void DeleteFatEntryChain(in SubEntryInfo info)
        {
            var fileEntry = LocateFile(info);

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

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        // TODO: support directories
        var fromEntry = LocateFile(from);

        EternalFileSystemEntry toEntry;

        try
        {
            toEntry = LocateFile(to);
        }
        catch (EternalFileSystemException e) when (e.State == EternalFileSystemState.CantLocateSubEntry)
        {
            toEntry = CreateSubEntry(to, false);
        }

        using EternalFileSystemFileStream fromStream = new(_fileSystem, fromEntry.FatEntryReference);
        using EternalFileSystemFileStream toStream = new(_fileSystem, toEntry.FatEntryReference);

        fromStream.CopyTo(toStream);
        OverwriteFileEntry(toEntry.FatEntryReference, to.FatEntry, entry => new(fromEntry.Size, entry.SubEntryName, entry.FatEntryReference));
    }

    public override void WriteFile(in SubEntryInfo info, Stream source)
    {
        var fileEntry = LocateFile(info);

        using (EternalFileSystemFileStream fileStream = new(_fileSystem, fileEntry.FatEntryReference))
        {
            source.CopyTo(fileStream);
        }

        int length = (int)source.Length;
        OverwriteFileEntry(fileEntry.FatEntryReference, info.FatEntry, entry => new(length, entry.SubEntryName, entry.FatEntryReference));
    }

    public override Stream OpenEntry(EternalFileSystemFatEntry entry)
    {
        return new EternalFileSystemFileStream(_fileSystem, entry);
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
