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
}
