using System;

namespace EternalFS.Library.Utils;

[AttributeUsage(AttributeTargets.Method)]
public class ByteSpanAttribute : Attribute
{
    public string Value { get; }

    public ByteSpanAttribute(string value)
    {
        Value = value;
    }
}
