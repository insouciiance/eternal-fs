using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using EternalFS.Generator.Extensions;
using EternalFS.Generator.Utils;
using EternalFS.Commands;
using Microsoft.CodeAnalysis;

using static EternalFS.Generator.Utils.TemplatesHelper;

namespace EternalFS.Generator.Generators;

public partial class CommandGenerator
{
    private static string GenerateCommandImplementation(INamedTypeSymbol command)
    {
        var commandAttribute = command.GetAttribute<CommandAttribute>()!;
        var commandName = (string)commandAttribute.ConstructorArguments[0].Value!;
        bool needsFileSystem = (bool)commandAttribute.ConstructorArguments[1].Value!;

        var summaryAttribute = command.GetAttribute<CommandSummaryAttribute>();
        string? commandSummary = (string?)summaryAttribute?.ConstructorArguments[0].Value;
        
        string commandDeclarationName = command.ToDisplayString(CommonFormats.Declaration);

        return $@"
using EternalFS.Commands;
using EternalFS.Library.Utils;

namespace {command.ContainingNamespace};

/// <summary>
/// {commandSummary}
/// </summary>
partial {GetTypeKindString(command)} {commandDeclarationName} : Singleton<{commandDeclarationName}>, ICommand
{{
    static CommandInfo ICommand.Info {{ get; }} = new(""{commandName}"", {needsFileSystem.ToString().ToLower()})
    {{
        Documentation = {GetDocumentation()}
    }};
}}";

        string GetDocumentation()
        {
            if (commandSummary is null)
                return "null";

            var argumentsAttributes = command.GetAttributes<CommandArgumentAttribute>();
            
            return
$$""" 
CommandDocumentation.CreateBuilder()
            .SetSummary("{{commandSummary}}")
            {{string.Join("\n", argumentsAttributes.Select(AddArgument))}}
            .ToDocumentation()
""";

            string AddArgument(AttributeData data)
            {
                var name = (string)data.ConstructorArguments[0].Value!;
                var description = (string)data.ConstructorArguments[1].Value!;
                var required = (bool)data.ConstructorArguments[2].Value!;

                return $@".AddArgument(new CommandDocumentation.Argument(""{name}"", ""{description}"", {(required ? "true" : "false")}))";
            }
        }
    }

    private static string GenerateCommandManagerType(ImmutableArray<INamedTypeSymbol> commands, string commandManagerTypeName)
    {
        IList<string> usings = new HashSet<string>(CollectUsings(commands))
        {
            "EternalFS.Commands.Diagnostics",
            "EternalFS.Commands.Extensions",
            "EternalFS.Library.Extensions",
            "EternalFS.Library.Filesystem",
            "EternalFS.Library.Utils",
            typeof(int).Namespace,
            typeof(Encoding).Namespace,
            typeof(List<>).Namespace,
            typeof(CommandAttribute).Namespace,
            typeof(Stream).Namespace,
            typeof(ArrayPool<>).Namespace
        }.OrderUsings();

        return $@"
{string.Join("\n", usings.Select(u => $"using {u};"))}

#nullable enable

public static partial class {commandManagerTypeName}
{{
    private const int MAX_COMMAND_LENGTH = 4096;

{string.Join("\n\n", commands
    .Select(c => $@"    private static ReadOnlySpan<byte> {GetCommandSpanName(c)} => ""{GetCommandName(c)}""u8;"))}

    static partial void PreprocessCommand(ref CommandExecutionContext context, ref CommandExecutionResult? result);
    
    static partial void PostProcessCommand(ref CommandExecutionContext context, ref CommandExecutionResult result);

    public static CommandExecutionResult ExecuteCommand(Stream source, ref CommandExecutionContext context)
    {{
        byte[] buffer = ArrayPool<byte>.Shared.Rent(MAX_COMMAND_LENGTH);
        buffer.AsSpan().Fill(0);

        source.Read(buffer);

        // just skip if there is no input
        if (buffer.AsSpan().TrimEnd(ByteSpanHelper.Null()).SequenceEqual(ReadOnlySpan<byte>.Empty))
            return new();

        context.Reader = new Utf8CommandReader(buffer);
        
        ReadOnlySpan<byte> commandSpan = context.Reader.ReadCommandName();

        CommandExecutionResult? result = null;

        try
        {{
	        PreprocessCommand(ref context, ref result);

            result = commandSpan switch
            {{
                _ when result is not null => result,
{string.Join("\n", commands.Select(SwitchCommand))}
                _ => throw new CommandExecutionException(CommandExecutionState.CommandNotFound, Encoding.UTF8.GetString(commandSpan))
            }};
        }}
        catch (Exception e)
        {{
            string message = e.Message!;

#if DEBUG
            message += Environment.NewLine + e.StackTrace;
#endif

            context.Writer.Error(message);

            result = CommandExecutionResult.Default;
        }}
        finally
        {{
		    PostProcessCommand(ref context, ref result!);
            ArrayPool<byte>.Shared.Return(buffer);
        }}

        return result;
    }}
}}";

        static string SwitchCommand(INamedTypeSymbol command)
        {
            var nameDisplayFormat = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

            string commandDeclarationName = command.ToDisplayString(nameDisplayFormat);

            return $@"                {GetCommandSwitchCase(command)} => {commandDeclarationName}.Instance.Execute(ref context),";
        }

        static string GetCommandSwitchCase(INamedTypeSymbol command)
        {
            string spanName = GetCommandSpanName(command);
            return $"var s when s.SequenceEqual({spanName})";
        }

        static string GetCommandSpanName(INamedTypeSymbol command)
        {
            var commandName = GetCommandName(command);
            return $"{commandName}Span";
        }
    }

    private static string GenerateCommandManagerCommandInfos(ImmutableArray<INamedTypeSymbol> commands, string commandManagerTypeName)
    {
        IList<string> usings = new HashSet<string>(CollectUsings(commands))
        {
            typeof(CommandAttribute).Namespace,
            typeof(CommandAttribute).Namespace + ".Utils",
            typeof(List<>).Namespace
        }.OrderUsings();

        return $@"
{string.Join("\n", usings.Select(u => $"using {u};"))}

#nullable enable

public static partial class {commandManagerTypeName}
{{
    public static readonly Dictionary<string, CommandInfo> CommandInfos = new()
    {{
{string.Join(",\n", commands.Select(c => $@"        {{ ""{GetCommandName(c)}"", {GetCommandInfo(c)} }}"))}
    }};
}}";
        static string GetCommandInfo(INamedTypeSymbol command) => $@"CommandHelper.GetInfo<{command.Name}>()";
    }

    private static string GetCommandName(INamedTypeSymbol command)
    {
        var commandAttribute = command.GetAttribute<CommandAttribute>()!;
        var commandName = (string)commandAttribute.ConstructorArguments[0].Value!;
        return commandName;
    }
}
