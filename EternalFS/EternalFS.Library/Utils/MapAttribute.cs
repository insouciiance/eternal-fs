using System;

namespace EternalFS.Library.Utils;

[AttributeUsage(AttributeTargets.Field)]
public class MapAttribute : Attribute
{
    public MapAttribute(string message) { }
}
