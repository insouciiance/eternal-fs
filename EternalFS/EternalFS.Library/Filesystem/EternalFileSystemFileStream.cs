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

            int clusterOffset = EternalFileSystemHelper.GetClusterOffset(_fileSystem, entry);

            int initialPosition = clusterOffset + _currentClusterIndex;
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

        _fileSystemStream.Seek(EternalFileSystemHelper.GetClusterOffset(_fileSystem, entry), SeekOrigin.Begin);
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

        if (nextEntry == _fatTerminator || nextEntry == EternalFileSystemMounter.EmptyCluster)
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
                int tailEntryOffset = EternalFileSystemHelper.GetFatEntryOffset(tailEntry);
                _fileSystemStream.Seek(tailEntryOffset, SeekOrigin.Begin);
                tailEntry = _fileSystemStream.MarshalReadStructure<EternalFileSystemFatEntry>();
            } while (tailEntry != _fatTerminator);

            _fileSystemStream.Position -= 2;
            _fileSystemStream.MarshalWriteStructure(entry);
        }

        void WriteTerminator(in EternalFileSystemFatEntry entry)
        {
            int entryOffset = EternalFileSystemHelper.GetFatEntryOffset(entry);
            _fileSystemStream.Seek(entryOffset, SeekOrigin.Begin);
            _fileSystemStream.MarshalWriteStructure(EternalFileSystemMounter.FatTerminator);
        }
    }

    private long GetPosition() => (_fatEntries.Count - 1) * EternalFileSystemMounter.CLUSTER_SIZE_BYTES + _currentClusterIndex;
}
