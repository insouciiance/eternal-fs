using System;

namespace EternalFS.Library.Utils;

/// <summary>
/// Maps the annotated enum value to the specified string in <see cref="IStringMap{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MapAttribute : Attribute
{
    public MapAttribute(string message) { }
}
