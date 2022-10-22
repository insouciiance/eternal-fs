﻿using System;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Extensions;

public static class SpanExtensions
{
    public static ReadOnlySpan<byte> SplitIndex(in this ReadOnlySpan<byte> span, in ReadOnlySpan<byte> delimiter, int index = 0)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        int startIndex = 0;
        int endIndex = 0;

        for (int i = 0; i <= index; i++)
        {
            if (endIndex == startIndex - 1)
                return ReadOnlySpan<byte>.Empty;

            startIndex = endIndex;

            if (i > 0)
                startIndex += delimiter.Length;

            endIndex = span[startIndex..].IndexOf(delimiter) + startIndex;
        }

        return endIndex != startIndex - 1 ? span[startIndex..endIndex] : span[startIndex..].TrimEnd(ByteSpanHelper.Null());
    }

    public static ReadOnlySpan<byte> SplitIndex(in this ReadOnlySpan<byte> span, int index = 0)
        => SplitIndex(span, ByteSpanHelper.Space(), index);
}