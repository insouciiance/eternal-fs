using System;
using System.Diagnostics.CodeAnalysis;

namespace EternalFS.Library.Utils;

/// <summary>
/// Represents a type that maps the enum's values to corresponding strings (provided by <see cref="MapAttribute"/>).
/// </summary>
public interface IStringMap<T>
    where T : unmanaged, Enum
{
    static abstract string GetString(T key);

    static abstract bool TryGetString(T key, [MaybeNullWhen(false)] out string value);
}
