using System;

namespace EternalFS.Library.Utils;

/// <summary>
/// Indicates an enum for which an implementation of <see cref="IStringMap{T}"/> will be generated.
/// </summary>
/// <remarks>
/// The generated implementation of <see cref="IStringMap{T}"/> will have a suffix <c>Map</c>,
/// e.g., an enum <c>MyEnum</c> will have a generated map <c>MyEnumMap</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Enum)]
public class StringMapAttribute : Attribute { }
