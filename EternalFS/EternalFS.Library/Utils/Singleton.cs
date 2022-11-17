namespace EternalFS.Library.Utils;

/// <summary>
/// Represents a singleton.
/// </summary>
public class Singleton<T>
    where T : Singleton<T>, new()
{
    public static T Instance { get; } = new();
}
