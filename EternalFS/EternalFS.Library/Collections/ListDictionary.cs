using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace EternalFS.Library.Collections;

public class ListDictionary<TKey, TValue> : IDictionary<TKey, List<TValue>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, List<TValue>> _dictionary = new();

    public ICollection<TKey> Keys => _dictionary.Keys;

    public ICollection<List<TValue>> Values => _dictionary.Values;

    public int KeyCount => _dictionary.Count;

    int ICollection<KeyValuePair<TKey, List<TValue>>>.Count => _dictionary.Count;

    public bool IsReadOnly => false;

    public void Clear()
        => _dictionary.Clear();

    public void Add(TKey key, List<TValue> value)
        => _dictionary.Add(key, value);

    public void Add(TKey key, TValue value)
    {
        ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, key, out _);
        list ??= new();
        list.Add(value);
    }

    public bool ContainsKey(TKey key)
        => _dictionary.ContainsKey(key);

    public bool Remove(TKey key)
        => _dictionary.Remove(key);

    public bool Remove(TKey key, TValue value)
    {
        if (!_dictionary.TryGetValue(key, out var list))
            return false;

        return list.Remove(value);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out List<TValue> value)
        => _dictionary.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    void ICollection<KeyValuePair<TKey, List<TValue>>>.Add(KeyValuePair<TKey, List<TValue>> item)
        => ((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).Add(item);

    bool ICollection<KeyValuePair<TKey, List<TValue>>>.Contains(KeyValuePair<TKey, List<TValue>> item)
        => ((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).Contains(item);

    void ICollection<KeyValuePair<TKey, List<TValue>>>.CopyTo(KeyValuePair<TKey, List<TValue>>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).CopyTo(array, arrayIndex);

    bool ICollection<KeyValuePair<TKey, List<TValue>>>.Remove(KeyValuePair<TKey, List<TValue>> item)
        => ((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).Remove(item);

    public List<TValue> this[TKey key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }
}
