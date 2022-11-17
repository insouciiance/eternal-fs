using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Utils;

/// <summary>
/// Provides helper methods to access arguments stored inside <see cref="ReadOnlySpan{T}"/>.
/// </summary>
public static class ArgumentsHelper
{
    public static bool TryGetArgumentValue(in ReadOnlySpan<byte> span, in ReadOnlySpan<byte> argumentName, out ReadOnlySpan<byte> value)
    {
        int nameIndex = span.IndexOf(argumentName);

        if (nameIndex == -1)
        {
            value = ReadOnlySpan<byte>.Empty;
            return false;
        }

        ReadOnlySpan<byte> nameParameterSpan = span[nameIndex..].SplitIndex(ByteSpanHelper.Space());
        value = nameParameterSpan.SplitIndex(ByteSpanHelper.Equals(), 1);
        return true;
    }
}
