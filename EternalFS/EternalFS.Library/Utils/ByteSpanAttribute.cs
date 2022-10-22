using System;

namespace EternalFS.Library.Utils;

[AttributeUsage(AttributeTargets.Method)]
public class ByteSpanAttribute : Attribute
{
    public ByteSpanAttribute(string value) { }
}
