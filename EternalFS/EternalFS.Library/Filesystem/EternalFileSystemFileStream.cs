﻿using System;
using System.Collections.Generic;
using System.IO;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem;

/// <summary>
/// Represents a <see cref="Stream"/> for an <see cref="EternalFileSystem"/>.
/// Allows read and write operations under the given <see cref="EternalFileSystem"/>
/// and switches underlying clusters seamlessly.
/// </summary>
/// <remarks>
/// It may also create new clusters if the content being written is not enough to fit into
/// already allocated clusters.
/// </remarks>
public class EternalFileSystemFileStream : Stream
{
    private readonly EternalFileSystem _fileSystem;

    private readonly Stream _fileSystemStream;

    private readonly List<EternalFileSystemFatEntry> _fatEntries = new();

    private byte[] _currentCluster = null!;

    private int _currentClusterIndex;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => 0;

    public override long Position { get => GetPosition(); set => Seek(value); }

    public EternalFileSystemFileStream(EternalFileSystem fileSystem, EternalFileSystemFatEntry fatEntry)
    {
        _fileSystem = fileSystem;
        _fileSystemStream = fileSystem.GetStream();
        _fatEntries.Add(fatEntry);

        _fileSystemStream.Seek(EternalFileSystemHeader.HeaderSize, SeekOrigin.Begin);

        InitCurrentCluster();
    }

    public override void Flush() => _fileSystemStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        int initialCount = count;
        int bytesTotal = 0;

        do
        {
            using MemoryStream ms = new(_currentCluster, _currentClusterIndex, _currentCluster.Length - _currentClusterIndex);
            int bytesRead = ms.Read(buffer, offset, count);

            bytesTotal += bytesRead;
            _currentClusterIndex += bytesRead;
            offset += bytesRead;
            
            count -= bytesRead;

            if (_currentClusterIndex == EternalFileSystemMounter.CLUSTER_SIZE_BYTES && !TryEnterNextCluster())
                break;

        } while (bytesTotal < initialCount);

        return bytesTotal;
    }

    public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        return origin switch
        {
            SeekOrigin.Begin => SeekFromBegin(),
            SeekOrigin.Current => Seek(Position + offset),
            _ => throw new NotSupportedException()
        };

        long SeekFromBegin()
        {
            int clusterIndex = (int)offset / EternalFileSystemMounter.CLUSTER_SIZE_BYTES;

            _fatEntries.RemoveRange(1, _fatEntries.Count - 1);

            _currentClusterIndex = 0;

            InitCurrentCluster();

            for (int i = 0; i < clusterIndex; i++)
                TryEnterNextCluster();

            _currentClusterIndex += (int)offset % EternalFileSystemMounter.CLUSTER_SIZE_BYTES;

            return Position;
        }
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_fatEntries.Count == 0)
            return;

        int bytesTotal = 0;

        while (true)
        {
            EternalFileSystemFatEntry entry = _fatEntries[^1];

            long clusterOffset = EternalFileSystemHelper.GetClusterOffset(_fileSystem.Size, entry);

            long initialPosition = clusterOffset + _currentClusterIndex;
            _fileSystemStream.Seek(initialPosition, SeekOrigin.Begin);
            _fileSystemStream.Write(buffer, offset, Math.Min(count, EternalFileSystemMounter.CLUSTER_SIZE_BYTES - _currentClusterIndex));

            int bytesWritten = (int)(_fileSystemStream.Position - initialPosition);
            bytesTotal += bytesWritten;
            _currentClusterIndex += bytesWritten;

            if (offset + count == bytesTotal || !TryEnterNextCluster(true))
                break;

            offset += bytesWritten;
            count -= bytesWritten;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _fileSystemStream.Dispose();
    }

    private void InitCurrentCluster()
    {
        if (_fatEntries.Count == 0)
            return;

        EternalFileSystemFatEntry entry = _fatEntries[^1];

        _fileSystemStream.Seek(EternalFileSystemHelper.GetClusterOffset(_fileSystem.Size, entry), SeekOrigin.Begin);
        _currentCluster = new byte[EternalFileSystemMounter.CLUSTER_SIZE_BYTES];
        _currentClusterIndex = 0;
        _fileSystemStream.Read(_currentCluster, 0, _currentCluster.Length);
    }

    private bool TryEnterNextCluster(bool createNewCluster = false)
    {
        if (_fatEntries.Count == 0)
            return false;

        EternalFileSystemFatEntry entry = _fatEntries[^1];
        EternalFileSystemFatEntry nextEntry;

        using Stream stream = _fileSystem.GetStream();
        stream.Seek(EternalFileSystemHelper.GetFatEntryOffset(entry), SeekOrigin.Begin);
        nextEntry = stream.MarshalReadStructure<EternalFileSystemFatEntry>();

        if (nextEntry == EternalFileSystemMounter.FatTerminator || nextEntry == EternalFileSystemMounter.EmptyCluster)
        {
            if (!createNewCluster || !TryAllocateNewCluster(out nextEntry))
                return false;
        }

        _fileSystemStream.Seek(EternalFileSystemHelper.GetFatEntryOffset(nextEntry), SeekOrigin.Begin);
        _fatEntries.Add(nextEntry);

        InitCurrentCluster();

        return true;
    }

    private bool TryAllocateNewCluster(out EternalFileSystemFatEntry entry)
    {
        if (EternalFileSystemHelper.TryAllocateNewFatEntry(_fileSystem, out entry))
        {
            UpdateTailEntry(entry);
            WriteTerminator(entry);
            return true;
        }

        entry = default;
        return false; // we should never realistically reach this point

        void UpdateTailEntry(in EternalFileSystemFatEntry entry)
        {
            EternalFileSystemFatEntry tailEntry = _fatEntries[^1];

            do
            {
                long tailEntryOffset = EternalFileSystemHelper.GetFatEntryOffset(tailEntry);
                _fileSystemStream.Seek(tailEntryOffset, SeekOrigin.Begin);
                tailEntry = _fileSystemStream.MarshalReadStructure<EternalFileSystemFatEntry>();
            } while (tailEntry != EternalFileSystemMounter.FatTerminator);

            _fileSystemStream.Position -= EternalFileSystemFatEntry.EntrySize;
            _fileSystemStream.MarshalWriteStructure(entry);
        }

        void WriteTerminator(in EternalFileSystemFatEntry entry)
        {
            long entryOffset = EternalFileSystemHelper.GetFatEntryOffset(entry);
            _fileSystemStream.Seek(entryOffset, SeekOrigin.Begin);
            _fileSystemStream.MarshalWriteStructure(EternalFileSystemMounter.FatTerminator);
        }
    }

    private long GetPosition() => (_fatEntries.Count - 1) * EternalFileSystemMounter.CLUSTER_SIZE_BYTES + _currentClusterIndex;
}
