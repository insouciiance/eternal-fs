using System.Collections.Immutable;
using System.Linq;
using EternalFS.Generator.Extensions;
using EternalFS.Library.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        var commandStatesProvider = context.CompilationProvider.Select(
            static (compilation, _) => compilation.GetTypeByMetadataName(typeof(CommandExecutionState).FullName));

        context.RegisterSourceOutput(commandsProvider, static (context, commands) =>
        {
            foreach (var command in commands)
                context.AddFileSource($"{command.Name}.g.cs", GenerateCommandImplementation(command));
        });

        context.RegisterSourceOutput(
            commandsProvider.Combine(commandManagerTypeNameProvider),
            static (context, data) =>
            {
                var (commands, managerName) = data;

                context.AddFileSource($"{managerName}.g.cs", GenerateCommandManagerType(commands, managerName));
                context.AddFileSource($"{managerName}.infos.g.cs", GenerateCommandManagerCommandInfos(commands, managerName));
            });

        context.RegisterSourceOutput(
            commandStatesProvider.Combine(commandManagerTypeNameProvider),
            static (context, data) =>
            {
                var (commandStates, managerName) = data;

                context.AddFileSource($"{managerName}.states.g.cs", GenerateCommandManagerCommandStates(commandStates, managerName));
            });
    }
}
