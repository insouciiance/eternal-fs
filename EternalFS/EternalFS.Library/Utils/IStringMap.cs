using System;

namespace EternalFS.Library.Utils;

public interface IStringMap<T>
    where T : unmanaged, Enum
{
    static abstract string GetString(T key);

    static abstract bool TryGetString(T key, out string value);
}
