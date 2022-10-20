namespace EternalFS.Library.Extensions;

public static class StringExtensions
{
    public static string[] SplitIntoComponents(this string filePath)
    {
        return filePath.Split('\\');
    }
}
