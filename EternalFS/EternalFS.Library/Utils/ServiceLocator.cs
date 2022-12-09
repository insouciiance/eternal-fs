using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EternalFS.Library.Utils;

public class ServiceLocator
{
    private readonly Dictionary<Type, object> _services = new();

    public void Add<T>(T value)
        where T : notnull
        => _services.Add(typeof(T), value);

    public void Set<T>(T value)
        where T : notnull
        => _services[typeof(T)] = value;

    public bool Remove<T>()
        => _services.Remove(typeof(T));

    public T Get<T>()
        => (T)_services[typeof(T)];

    public bool TryGet<T>([MaybeNullWhen(false)] out T value)
    {
        if (_services.TryGetValue(typeof(T), out var @object))
        {
            value = (T)@object;
            return true;
        }

        value = default;
        return false;
    }
}
