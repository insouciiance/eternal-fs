using System;

namespace EternalFS.Library.Utils;

public static partial class ByteSpanHelper
{
    [ByteSpan(" ")]
    public static partial ReadOnlySpan<byte> Space();

    [ByteSpan("\0")]
    public static partial ReadOnlySpan<byte> Null();

    [ByteSpan(".")]
    public static partial ReadOnlySpan<byte> Period();

    [ByteSpan("..")]
    public static partial ReadOnlySpan<byte> ParentDirectory();

    [ByteSpan("=")]
    public static partial ReadOnlySpan<byte> Equals();

    [ByteSpan("/")]
    public static partial ReadOnlySpan<byte> ForwardSlash();
}
