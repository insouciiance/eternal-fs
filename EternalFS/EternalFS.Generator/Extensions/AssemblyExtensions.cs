using System.Linq;
using Microsoft.CodeAnalysis;

namespace EternalFS.Generator.Extensions;

public static class AssemblyExtensions
{
    public static INamedTypeSymbol? GetSymbolByMetadataName(this IAssemblySymbol assembly, string fullName)
    {
        INamedTypeSymbol? result = assembly.GetTypeByMetadataName(fullName);

        if (result is not null)
            return result;

        foreach (var reference in assembly.Modules.SelectMany(m => m.ReferencedAssemblySymbols))
        {
            result = reference.GetTypeByMetadataName(fullName);

            if (result is not null)
                break;
        }

        return result;
    }
}
