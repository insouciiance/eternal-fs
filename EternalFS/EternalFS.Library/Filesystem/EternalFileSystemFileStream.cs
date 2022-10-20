using System;
using System.Collections.Generic;
using System.IO;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Filesystem;

public class EternalFileSystemFileStream : Stream
{
    private readonly EternalFileSystem _fileSystem;

    private readonly Stream _fileSystemStream;

    private readonly List<EternalFileSystemFatEntry> _fatEntries = new();

    private readonly EternalFileSystemFatEntry _fatTerminator;

    private byte[] _currentCluster = null!;

    private int _currentClusterIndex;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => 0;

    public override long Position { get => GetPosition(); set => Seek(value, SeekOrigin.Begin); }

    public EternalFileSystemFileStream(EternalFileSystem fileSystem, EternalFileSystemFatEntry fatEntry)
    {
        _fileSystem = fileSystem;
        _fileSystemStream = fileSystem.GetStream();
        _fatEntries.Add(fatEntry);

        _fileSystemStream.Seek(EternalFileSystemHeader.HeaderSize, SeekOrigin.Begin);
        _fatTerminator = _fileSystemStream.MarshalReadStructure<EternalFileSystemFatEntry>();

        InitCurrentCluster();
    }

    public override void Flush() => _fileSystemStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = 0;

        do
        {
            using MemoryStream ms = new(_currentCluster, _currentClusterIndex, _currentCluster.Length - _currentClusterIndex);
            _currentClusterIndex += ms.Read(buffer, offset, count);

            bytesRead += _currentClusterIndex;

            if (_currentClusterIndex == EternalFileSystem.CLUSTER_SIZE_BYTES - 1 && !TryEnterNextCluster())
                break;

        } while (bytesRead < count);

        return bytesRead;
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
            int clusterIndex = (int)offset / EternalFileSystem.CLUSTER_SIZE_BYTES;

            _fatEntries.RemoveRange(1, _fatEntries.Count - 1);

            _currentClusterIndex = 0;

            for (int i = 0; i < clusterIndex; i++)
            {
                InitCurrentCluster();
                TryEnterNextCluster();
            }

            _currentClusterIndex += (int)offset % EternalFileSystem.CLUSTER_SIZE_BYTES;

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

        for (int i = 0; i < buffer.Length; i += EternalFileSystem.CLUSTER_SIZE_BYTES)
        {
            EternalFileSystemFatEntry entry = _fatEntries[^1];

            int clusterOffset = GetClusterOffset(entry);

            int initialPosition = clusterOffset + _currentClusterIndex;
            _fileSystemStream.Seek(initialPosition, SeekOrigin.Begin);
            _fileSystemStream.Write(buffer, offset, count);

            _currentClusterIndex += (int)(_fileSystemStream.Position - initialPosition);

            if (buffer.Length < i + EternalFileSystem.CLUSTER_SIZE_BYTES || !TryEnterNextCluster())
                break;

            offset += EternalFileSystem.CLUSTER_SIZE_BYTES;
            count -= EternalFileSystem.CLUSTER_SIZE_BYTES;
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

        _fileSystemStream.Seek(GetClusterOffset(entry), SeekOrigin.Begin);
        _currentCluster = new byte[EternalFileSystem.CLUSTER_SIZE_BYTES];
        _currentClusterIndex = 0;
        _fileSystemStream.Read(_currentCluster, 0, _currentCluster.Length);
    }

    private bool TryEnterNextCluster(bool createNewCluster = false)
    {
        if (_fatEntries.Count == 0)
            return false;

        EternalFileSystemFatEntry entry = _fatEntries[^1];

        if (entry == _fatTerminator && (!createNewCluster || !TryAllocateNewCluster(out entry)))
            return false;

        _fileSystemStream.Seek(GetFatEntryOffset(entry), SeekOrigin.Begin);
        EternalFileSystemFatEntry nextEntry = _fileSystemStream.MarshalReadStructure<EternalFileSystemFatEntry>();
        _fatEntries.Add(nextEntry);

        InitCurrentCluster();

        return true;
    }

    private int GetClusterOffset(EternalFileSystemFatEntry entry)
    {
        return EternalFileSystemHeader.HeaderSize + 2 +
            _fileSystem.ClustersCount * EternalFileSystem.FAT_ENTRY_SIZE_BYTES +
            entry * EternalFileSystem.CLUSTER_SIZE_BYTES;
    }

    private static int GetFatEntryOffset(EternalFileSystemFatEntry entry)
    {
        return EternalFileSystemHeader.HeaderSize + 2 +
            entry * EternalFileSystem.FAT_ENTRY_SIZE_BYTES;
    }

    private bool TryAllocateNewCluster(out EternalFileSystemFatEntry entry)
    {
        EternalFileSystemFatEntry currentEntry;

        _fileSystemStream.Seek(EternalFileSystemHeader.HeaderSize + 4, SeekOrigin.Begin);

        HashSet<EternalFileSystemFatEntry> allocatedEntries = new();

        while ((currentEntry = _fileSystemStream.MarshalReadStructure<EternalFileSystemFatEntry>()) != EternalFileSystemMounter.EmptyCluster)
            allocatedEntries.Add(currentEntry);

        if (allocatedEntries.Count == _fileSystem.ClustersCount)
        {
            entry = default;
            return false; // TODO: handle out of memory
        }

        for (ushort i = 0; i < 0xFFFF; i++)
        {
            currentEntry = new(i);

            if (allocatedEntries.Contains(currentEntry))
                continue;

            _fileSystemStream.Position -= 2;
            _fileSystemStream.MarshalWriteStructure(currentEntry);

            UpdateTailEntry();
            entry = currentEntry;
            return true;
        }

        entry = default;
        return false; // we should never realistically reach this point

        void UpdateTailEntry()
        {
            EternalFileSystemFatEntry tailEntry = _fatEntries[0];

            do
            {
                int tailEntryOffset = GetFatEntryOffset(tailEntry);
                _fileSystemStream.Seek(tailEntryOffset, SeekOrigin.Begin);
                tailEntry = _fileSystemStream.MarshalReadStructure<EternalFileSystemFatEntry>();
            } while (tailEntry != _fatTerminator);

            _fileSystemStream.Position -= 2;
            _fileSystemStream.MarshalWriteStructure(currentEntry);
        }
    }

    private long GetPosition() => (_fatEntries.Count - 1) * EternalFileSystem.CLUSTER_SIZE_BYTES + _currentClusterIndex;
}
