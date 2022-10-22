namespace EternalFS.Library.Utils;

public class Singleton<T>
    where T : Singleton<T>, new()
{
    public static T Instance { get; } = new();
}
