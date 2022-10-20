using Microsoft.CodeAnalysis;

namespace EternalFS.Generator.Utils;

public static class CommonFormats
{
    public static SymbolDisplayFormat Declaration = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
}
