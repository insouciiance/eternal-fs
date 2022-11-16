using System.Linq;
using EternalFS.Generator.Extensions;
using EternalFS.Library.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EternalFS.Generator.Generators;

[Generator]
public partial class StringMapGenerator : IIncrementalGenerator
{
    public const string SUFFIX = "Map";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumsProvider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node.IsKind(SyntaxKind.EnumDeclaration),
            (node, _) => (INamedTypeSymbol)node.SemanticModel.GetDeclaredSymbol(node.Node)!);

        var stringMapsProvider = enumsProvider.Where(SymbolExtensions.HasAttribute<StringMapAttribute>);

        context.RegisterSourceOutput(
            stringMapsProvider,
            (context, stringMapSymbol) =>
            {
                context.AddFileSource($"{stringMapSymbol}Map.g.cs", GenerateStringMap(stringMapSymbol));
            });
    }
}
