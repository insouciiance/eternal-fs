using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using EternalFS.Generator.Extensions;
using EternalFS.Generator.Utils;
using EternalFS.Library.Commands;
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

        var commandDocAttribute = command.GetAttribute<CommandDocAttribute>();
        string? commandSummary = (string?)commandDocAttribute?.ConstructorArguments[0].Value;
        
        string commandDeclarationName = command.ToDisplayString(CommonFormats.Declaration);

        return $@"
using EternalFS.Library.Commands;
using EternalFS.Library.Utils;

namespace {command.ContainingNamespace};

/// <summary>
/// {commandSummary}
/// </summary>
partial {GetTypeKindString(command)} {commandDeclarationName} : Singleton<{commandDeclarationName}>, ICommand
{{
    static CommandInfo ICommand.Info {{ get; }} = new(""{commandName}"", {needsFileSystem.ToString().ToLower()})
    {{
        Documentation = {(commandSummary is { } ? @$"new(""{commandSummary}"")" : "null")}
    }};
}}
";
    }

    private static string GenerateCommandManagerType(ImmutableArray<INamedTypeSymbol> commands, string commandManagerTypeName)
    {
        IList<string> usings = new HashSet<string>(CollectUsings(commands))
        {
            "EternalFS.Library.Filesystem",
            "EternalFS.Library.Utils",
            typeof(int).Namespace,
            typeof(Encoding).Namespace,
            typeof(List<>).Namespace,
            typeof(CommandAttribute).Namespace,
            typeof(Stream).Namespace
        }.OrderUsings();

        return $@"
{string.Join("\n", usings.Select(u => $"using {u};"))}

#nullable enable

public static partial class {commandManagerTypeName}
{{
    private const int MAX_COMMAND_LENGTH = 4096;

{string.Join("\n\n", commands
    .Select(c => $"    private static ReadOnlySpan<byte> {GetCommandSpanName(c)} => new byte[] {{ {GetCommandSpanBytes(c)} }};"))}

    static partial void PreprocessCommand(ref CommandExecutionContext context, in ReadOnlySpan<byte> commandSpan, ref CommandExecutionResult? result);

    public static CommandExecutionResult ExecuteCommand(Stream source, ref CommandExecutionContext context)
    {{
        Span<byte> buffer = new byte[MAX_COMMAND_LENGTH];
    
        source.Read(buffer);

        int spaceIndex = buffer.IndexOf(ByteSpanHelper.Space());

        if (spaceIndex == -1)
            spaceIndex = buffer.IndexOf(ByteSpanHelper.Null());

        // just skip if there is no input
        if (buffer.TrimEnd(ByteSpanHelper.Null()).SequenceEqual(ReadOnlySpan<byte>.Empty))
            return new();

        ReadOnlySpan<byte> commandSpan = buffer[..spaceIndex];
        context.ValueSpan = buffer[(spaceIndex + 1)..];

        CommandExecutionResult? result = null;

        PreprocessCommand(ref context, commandSpan, ref result);

        try
        {{
            return commandSpan switch
            {{
                _ when result is not null => result,
{string.Join("\n", commands.Select(SwitchCommand))}
                _ => HandleDefault(ref context, $@""Unable to process """"{{Encoding.UTF8.GetString(commandSpan)}}"""": command not found."")
            }};
        }}
        catch (Exception e)
        {{
            return HandleDefault(ref context, e.Message);
        }}

        CommandExecutionResult HandleDefault(ref CommandExecutionContext context, string message)
        {{
            context.Writer.WriteLine(message);
            return new() {{ ExitCode = -1 }};
        }}
    }}
}}
";

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

        static string GetCommandSpanBytes(INamedTypeSymbol command)
        {
            var commandName = GetCommandName(command);
            return string.Join(", ", Encoding.UTF8.GetBytes(commandName));
        }
    }

    private static string GenerateCommandManagerCommandInfos(ImmutableArray<INamedTypeSymbol> commands, string commandManagerTypeName)
    {
        IList<string> usings = new HashSet<string>(CollectUsings(commands))
        {
            typeof(CommandAttribute).Namespace,
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

    private static CommandInfo GetInfo<T>()
        where T : ICommand
        => T.Info;
}}
";
        static string GetCommandInfo(INamedTypeSymbol command) => $@"GetInfo<{command.Name}>()";
    }

    private static string GetCommandName(INamedTypeSymbol command)
    {
        var commandAttribute = command.GetAttribute<CommandAttribute>()!;
        var commandName = (string)commandAttribute.ConstructorArguments[0].Value!;
        return commandName;
    }
}
