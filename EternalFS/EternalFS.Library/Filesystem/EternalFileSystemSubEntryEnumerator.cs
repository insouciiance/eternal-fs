using System;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem;

internal ref struct EternalFileSystemSubEntryEnumerator
{
    private readonly ReadOnlySpan<byte> _path;

    private ReadOnlySpan<byte> _remaining;

    public ReadOnlySpan<byte> Current { get; private set; }

    public EternalFileSystemSubEntryEnumerator(ReadOnlySpan<byte> path)
    {
        _path = path;
        _remaining = _path;
    }

    public EternalFileSystemSubEntryEnumerator GetEnumerator() => new(_path);

    public bool MoveNext()
    {
        if (_remaining.Length == 0)
            return false;

        int nextIndex = _remaining.IndexOf(ByteSpanHelper.ForwardSlash());

        if (nextIndex == -1)
        {
            // the end is reached, one more entry is there though
            Current = _remaining;
            _remaining = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = _remaining[..nextIndex];
        _remaining = _remaining[(nextIndex + 1)..];
        return true;
    }

    public void Reset()
    {
        Current = default;
        _remaining = _path;
    }
}
