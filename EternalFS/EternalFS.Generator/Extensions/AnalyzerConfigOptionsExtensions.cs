using Microsoft.CodeAnalysis.Diagnostics;

namespace EternalFS.Generator.Extensions;

public static class AnalyzerConfigOptionsExtensions
{
    public static string? GetMSBuildProperty(
        this AnalyzerConfigOptions options,
        string name,
        string? defaultValue = null)
    {
        options.TryGetValue($"build_property.{name}", out var value);
        return value ?? defaultValue;
    }
}
