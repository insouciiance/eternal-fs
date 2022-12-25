using System;
using System.Diagnostics.CodeAnalysis;

namespace EternalFS.Commands;

public readonly ref struct CommandArgument
{
    public ReadOnlySpan<byte> Name { get; }

    [UnscopedRef]
    public ReadOnlySpan<byte> Value { get; }

    public readonly bool HasValue;

    public CommandArgument(ReadOnlySpan<byte> name)
    {
        Name = name;
    }

    public CommandArgument(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        Name = name;
        Value = value;
        HasValue = true;
    }
}
