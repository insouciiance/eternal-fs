using System.Collections.Immutable;
using System.Linq;
using EternalFS.Generator.Extensions;
using EternalFS.Library.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EternalFS.Generator.Generators;

[Generator]
public partial class CommandGenerator : IIncrementalGenerator
{
    private const string DEFAULT_COMMAND_MANAGER_TYPE_NAME = "CommandManager";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var commandManagerTypeNameProvider = context.AnalyzerConfigOptionsProvider.Select(
            static (o, _) => o.GlobalOptions.GetMSBuildProperty("CommandManagerTypeName", DEFAULT_COMMAND_MANAGER_TYPE_NAME)!);

        var commandsProvider = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is TypeDeclarationSyntax,
            static (syntax, _) =>
            {
                var symbol = (INamedTypeSymbol)syntax.SemanticModel.GetDeclaredSymbol(syntax.Node)!;
                return symbol.HasAttribute<CommandAttribute>() ? symbol : null;
            })
            .Where(static s => s is not null)
            .Collect()
            .Select(static (symbols, _) => symbols.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).ToImmutableArray());

        context.RegisterSourceOutput(commandsProvider, static (context, commands) =>
        {
            foreach (var command in commands)
                context.AddFileSource($"{command.Name}.g.cs", GenerateCommandImplementation(command));
        });

        context.RegisterSourceOutput(
            commandsProvider.Combine(commandManagerTypeNameProvider),
            static (context, data) =>
            {
                context.AddFileSource($"{data.Right}.g.cs", GenerateCommandManagerType(data.Left, data.Right));
                context.AddFileSource($"{data.Right}.doc.g.cs", GenerateCommandManagerDocumentation(data.Left, data.Right));
            });
    }
}
