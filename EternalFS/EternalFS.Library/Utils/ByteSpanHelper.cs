using System;

namespace EternalFS.Library.Utils;

/// <summary>
/// Provides widely used <see cref="ReadOnlySpan{T}"/>s.
/// </summary>
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

    [ByteSpan(@"\")]
    public static partial ReadOnlySpan<byte> BackSlash();

    [ByteSpan("#")]
    public static partial ReadOnlySpan<byte> Hash();

    [ByteSpan("%")]
    public static partial ReadOnlySpan<byte> Percent();

    [ByteSpan("&")]
    public static partial ReadOnlySpan<byte> Ampersand();

    [ByteSpan("{")]
    public static partial ReadOnlySpan<byte> LeftCurlyBracket();

    [ByteSpan("}")]
    public static partial ReadOnlySpan<byte> RightCurlyBracket();

    [ByteSpan("<")]
    public static partial ReadOnlySpan<byte> LeftAngleBracket();

    [ByteSpan(">")]
    public static partial ReadOnlySpan<byte> RightAngleBracket();

    [ByteSpan("*")]
    public static partial ReadOnlySpan<byte> Asterisk();

    [ByteSpan("?")]
    public static partial ReadOnlySpan<byte> QuestionMark();

    [ByteSpan("$")]
    public static partial ReadOnlySpan<byte> Dollar();

    [ByteSpan("!")]
    public static partial ReadOnlySpan<byte> ExclamationMark();

    [ByteSpan("'")]
    public static partial ReadOnlySpan<byte> SingleQuote();

    [ByteSpan(@"""")]
    public static partial ReadOnlySpan<byte> DoubleQuote();

    [ByteSpan(":")]
    public static partial ReadOnlySpan<byte> Colon();

    [ByteSpan("@")]
    public static partial ReadOnlySpan<byte> At();

    [ByteSpan("+")]
    public static partial ReadOnlySpan<byte> Plus();

    [ByteSpan("|")]
    public static partial ReadOnlySpan<byte> Pipe();

    [ByteSpan("-")]
    public static partial ReadOnlySpan<byte> Dash();
}
