using System.Collections.Immutable;
using System.Linq;
using EternalFS.Generator.Extensions;
using EternalFS.Library.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EternalFS.Generator.Generators;

[Generator]
public partial class ByteSpanGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typesProvider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node is TypeDeclarationSyntax,
            (syntax, _) =>
            {
                var symbol = (INamedTypeSymbol)syntax.SemanticModel.GetDeclaredSymbol(syntax.Node)!;
                return symbol.GetMembers().Any(m => m.HasAttribute<ByteSpanAttribute>()) ? symbol : null;
            })
            .Where(s => s is not null)
            .Collect()
            .Select((symbols, _) => symbols.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).ToImmutableArray());

        context.RegisterSourceOutput(typesProvider, (context, types) =>
        {
            foreach (var type in types)
                context.AddFileSource($"{type.Name}.g.cs", GenerateByteSpansForType(type));
        });
    }
}
