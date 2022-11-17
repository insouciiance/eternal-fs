using System;

namespace EternalFS.Library.Utils;

/// <summary>
/// Indicates a method that holds a string value in the form of a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <remarks>
/// The method must be partial and return <see cref="ReadOnlySpan{T}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class ByteSpanAttribute : Attribute
{
    public ByteSpanAttribute(string value) { }
}
