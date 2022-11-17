using EternalFS.Library.Utils;
using System;

namespace EternalFS.Library.Diagnostics;

/// <summary>
/// Represents an exception that uses <see cref="IStringMap{T}"/> internally to map error messages from enum values.
/// </summary>
public abstract class StringMapException<TMap, TEnum> : ApplicationException
    where TMap : IStringMap<TEnum>
    where TEnum : unmanaged, Enum
{
    private readonly object?[] _args;
    
    public TEnum State { get; init; }

    protected StringMapException(TEnum state, params object?[] args)
    {
        _args = args;
        State = state;
    }

    protected abstract string GetUnmappedMessage(TEnum state);

    public override string Message => TMap.TryGetString(State, out var message)
        ? string.Format(message, _args)
        : GetUnmappedMessage(State);
}
