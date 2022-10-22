using System.Linq;
using System.Text;
using EternalFS.Generator.Extensions;
using EternalFS.Generator.Utils;
using EternalFS.Library.Utils;
using Microsoft.CodeAnalysis;

using static EternalFS.Generator.Utils.TemplatesHelper;

namespace EternalFS.Generator.Generators;

public partial class ByteSpanGenerator
{
    private static string GenerateByteSpansForType(INamedTypeSymbol type)
    {
        var byteSpanMethods = type.GetMembers().OfType<IMethodSymbol>().Where(m => m.HasAttribute<ByteSpanAttribute>());

        string typeDeclarationName = type.ToDisplayString(CommonFormats.Declaration);

        return $@"
using System;

namespace {type.ContainingNamespace};

partial {GetTypeKindString(type)} {typeDeclarationName}
{{
{string.Join("\n\n", byteSpanMethods
    .Select(GetByteSpanMethod))}
}}
";

        static string GetByteSpanMethod(IMethodSymbol method)
        {
            var byteSpanAttribute = method.GetAttribute<ByteSpanAttribute>()!;
            string value = (string)byteSpanAttribute.ConstructorArguments[0].Value!;

            return $@"
    /// <summary>
    /// A <see cref=""ReadOnlySpan{{T}}"" /> that is equal to the string ""<c>{value}</c>"".
    /// </summary>
    {GetAccessibility(method)} {Static(method)} partial ReadOnlySpan<byte> {method.Name}() => new byte[] {{ {GetSpanBytes()} }};".TrimStart('\n', '\r');

            string GetSpanBytes() => string.Join(", ", Encoding.UTF8.GetBytes(value));
        }
    }
}
