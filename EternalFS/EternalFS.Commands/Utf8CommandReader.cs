using System;
using System.Buffers;
using EternalFS.Library.Utils;

namespace EternalFS.Commands;

public ref struct Utf8CommandReader
{
    public ReadOnlySpan<byte> OriginalSequence { get; }

    public bool IsFullyRead => _sequence.AsSpan().IndexOf(false) == -1;

    private readonly bool[] _sequence;

    private int _positionalIndex;

    public Utf8CommandReader(ReadOnlySpan<byte> sequence)
    {
        OriginalSequence = sequence;
        _positionalIndex = sequence.IndexOf(ByteSpanHelper.Space());
        _sequence = ArrayPool<bool>.Shared.Rent(OriginalSequence.Length);
        FillNulls();
    }

    public ReadOnlySpan<byte> ReadCommandName()
    {
        int spaceIndex = OriginalSequence.IndexOf(ByteSpanHelper.Space());

        ReadOnlySpan<byte> commandName;

        if (spaceIndex != -1)
        {
            commandName = OriginalSequence[..spaceIndex];
        }
        else
        {
            int nullIndex = OriginalSequence.IndexOf(ByteSpanHelper.Null());
            commandName = nullIndex != -1 ? OriginalSequence[..nullIndex] : OriginalSequence;
        }

        _sequence.AsSpan()[..commandName.Length].Fill(true);

        return commandName;
    }

    public bool TryReadNamedArgument(in ReadOnlySpan<byte> name, out CommandArgument argument)
    {
        int argumentIndex = OriginalSequence.IndexOf(name);

        if (argumentIndex == -1)
        {
            argument = default;
            return false;
        }

        return TryReadArgumentInternal(name, argumentIndex, out argument);
    }

    public bool TryReadPositionalArgument(out ReadOnlySpan<byte> value)
    {
        var initial = _positionalIndex != -1
            ? _positionalIndex
            : OriginalSequence.IndexOf(ByteSpanHelper.Null());

        if (initial == -1)
        {
            value = default;
            return false;
        }

        return TryReadPositionalArgumentInternal(OriginalSequence, initial + 1, true, out value);
    }

    private void FillNulls()
    {
        // treat nulls and spaces as if they are read already
        for (int i = 0; i < _sequence.Length; i++)
        {
            if (OriginalSequence[i] is (byte)'\0' or (byte)' ')
                _sequence[i] = true;
        }
    }

    private bool TryReadArgumentInternal(in ReadOnlySpan<byte> name, int initial, out CommandArgument argument)
    {
        int start = initial;
        int end = start;

        while (end <= OriginalSequence.Length)
        {
            if (end == OriginalSequence.Length || OriginalSequence[end] is (byte)' ' or 0)
            {
                ReadOnlySpan<byte> spaceSpan = OriginalSequence[start..end];

                _sequence.AsSpan()[initial..end].Fill(true);

                if (name.SequenceEqual(spaceSpan))
                {
                    argument = new(name);
                    return true;
                }

                argument = new(name, spaceSpan);
                return true;
            }

            byte current = OriginalSequence[end];

            if (current == '=')
            {
                if (!name.SequenceEqual(OriginalSequence[start..end]))
                {
                    argument = default;
                    return false;
                }

                _sequence.AsSpan()[start..(end + 1)].Fill(true);

                bool read = TryReadPositionalArgumentInternal(OriginalSequence, end + 1, false, out var value);
                argument = read ? new(name, value) : default;
                return read;
            }

            if (current == '"')
            {
                if (end == 0 || OriginalSequence[end - 1] != '=')
                {
                    argument = default;
                    return false;
                }

                ReadOnlySpan<byte> quotesSpan = ReadQuotesInternal(OriginalSequence, end);
                _sequence.AsSpan()[initial..(end + quotesSpan.Length)].Fill(true);

                argument = new(name, quotesSpan);
                return true;
            }

            end++;
        }

        argument = default;
        return false;
    }

    private bool TryReadPositionalArgumentInternal(ReadOnlySpan<byte> sequence, int initial, bool updatePosition, out ReadOnlySpan<byte> value)
    {
        int end = initial;

        while (end < sequence.Length)
        {
            byte current = sequence[end];

            if (current is (byte)' ' or (byte)'-' or 0 or (byte)'"')
            {
                if (current is (byte)'"')
                {
                    value = ReadQuotesInternal(sequence, end);
                    end = end + 1 + value.Length + 1;
                }
                else
                {
                    value = sequence[initial..end];
                }

                if (updatePosition)
                    _positionalIndex = end;

                _sequence.AsSpan()[initial..end].Fill(true);
                return true;
            }

            end++;
        }

        value = ReadOnlySpan<byte>.Empty;
        return false;
    }

    private static ReadOnlySpan<byte> ReadQuotesInternal(ReadOnlySpan<byte> sequence, int start)
    {
        if (sequence[start] != '"')
            throw new InvalidOperationException();

        start++;
        int end = start;

        while (end < sequence.Length)
        {
            if (sequence[end] == '"')
                return sequence[start..end];

            end++;
        }

        throw new InvalidOperationException();
    }

    public void Dispose()
    {
        ArrayPool<bool>.Shared.Return(_sequence);
    }
}
