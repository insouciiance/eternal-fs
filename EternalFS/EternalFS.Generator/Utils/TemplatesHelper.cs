using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace EternalFS.Generator.Utils;

internal static class TemplatesHelper
{
    public static string GetTypeKindString(INamedTypeSymbol symbol)
    {
        return symbol.IsValueType ? "struct" : "class";
    }

    public static ISet<string> CollectUsings(IEnumerable<ITypeSymbol> types)
    {
        return new HashSet<string>(types.Select(t => t.ContainingNamespace.ToDisplayString()));
    }

    public static IList<string> OrderUsings(this ISet<string> usings)
    {
        List<string> result = new();

        result.AddRange(usings.Where(u => u.StartsWith("System")).OrderBy(u => u));
        result.AddRange(usings.Where(u => !u.StartsWith("System")).OrderBy(u => u));

        return result;
    }

    public static string GetAccessibility(ISymbol symbol)
    {
        return symbol.DeclaredAccessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.NotApplicable => string.Empty,
            _ => throw new NotSupportedException()
        };
    }

    public static string Static(ISymbol symbol) => symbol.IsStatic ? "static" : string.Empty;
}
