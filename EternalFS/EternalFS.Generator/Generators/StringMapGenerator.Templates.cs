using EternalFS.Generator.Extensions;
using EternalFS.Library.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace EternalFS.Generator.Generators;

public partial class StringMapGenerator
{
    public string GenerateStringMap(INamedTypeSymbol stringMapSymbol)
    {
        return $$"""
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EternalFS.Library.Utils;

{{(stringMapSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : $"namespace {stringMapSymbol.ContainingNamespace};")}}
            
public class {{stringMapSymbol.Name + SUFFIX}} : IStringMap<{{stringMapSymbol}}>
{
    private static readonly Dictionary<{{stringMapSymbol}}, string> _stringMap = new()
    {
{{string.Join("\n", GetStringMap().Select(kvp => $$""""        { {{stringMapSymbol.Name}}.{{kvp.Key}},  {{kvp.Value}} },""""))}}
    };

    public static string GetString({{stringMapSymbol}} key) => _stringMap[key];

    public static bool TryGetString({{stringMapSymbol}} key, [MaybeNullWhen(false)] out string value) => _stringMap.TryGetValue(key, out value);
}
""";

        Dictionary<string, string> GetStringMap()
        {
            Dictionary<string, string> stringMap = new();

            foreach (var member in stringMapSymbol.GetMembers().OfType<IFieldSymbol>())
            {
                if (member.GetAttribute<MapAttribute>() is not { } map)
                    continue;

                string mapString = map.ConstructorArguments[0].ToCSharpString()!;

                stringMap.Add(member.Name, mapString);
            }

            return stringMap;
        }
    }
}
