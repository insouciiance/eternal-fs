using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace EternalFS.Generator.Extensions;

public static class SymbolExtensions
{
    public static bool HasAttribute<T>(this ISymbol symbol)
        where T : Attribute
        => symbol.GetAttribute<T>() is not null;

    public static AttributeData? GetAttribute<T>(this ISymbol symbol)
        where T : Attribute
    {
        var containingAssembly = symbol.ContainingAssembly;
        var attributeSymbol = containingAssembly.GetSymbolByMetadataName(typeof(T).FullName);

        foreach (var attributeData in symbol.GetAttributes())
        {
            if (attributeData.AttributeClass!.OriginalDefinition.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                return attributeData;
        }

        return null;
    }

    public static ImmutableArray<AttributeData> GetAttributes<T>(this ISymbol symbol)
        where T : Attribute
    {
        var builder = ImmutableArray.CreateBuilder<AttributeData>();

        var containingAssembly = symbol.ContainingAssembly;
        var attributeSymbol = containingAssembly.GetSymbolByMetadataName(typeof(T).FullName);

        foreach (var attributeData in symbol.GetAttributes())
        {
            if (attributeData.AttributeClass!.OriginalDefinition.Equals(attributeSymbol, SymbolEqualityComparer.Default))
            {
                builder.Add(attributeData);
            }
        }

        return builder.ToImmutable();
    }
}
