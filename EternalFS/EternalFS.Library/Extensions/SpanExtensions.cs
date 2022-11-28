using System;
using System.Text;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Extensions;

public static class SpanExtensions
{
    public static ReadOnlySpan<byte> SplitIndex(this scoped in ReadOnlySpan<byte> span, scoped in ReadOnlySpan<byte> delimiter, int index = 0)
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

        return endIndex != startIndex - 1 ? span[startIndex..endIndex] : span[startIndex..].TrimEndNull();
    }

    public static ReadOnlySpan<byte> SplitIndex(this in ReadOnlySpan<byte> span, int index = 0)
        => SplitIndex(span, ByteSpanHelper.Space(), index);

    public static bool Contains(this in ReadOnlySpan<byte> span, in ReadOnlySpan<byte> subSpan)
        => span.IndexOf(subSpan) != -1;

    public static string GetString(this in ReadOnlySpan<byte> span, Encoding? encoding = null)
        => (encoding ?? Encoding.UTF8).GetString(span);

    public static ReadOnlySpan<byte> TrimEndNull(this scoped in ReadOnlySpan<byte> span)
        => span.TrimEnd(ByteSpanHelper.Null());
}
