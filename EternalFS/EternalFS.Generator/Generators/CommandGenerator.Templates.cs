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
        
        var commandDocAttribute = command.GetAttribute<CommandDocAttribute>();
        var commandSummary = (string?)commandDocAttribute?.ConstructorArguments[0].Value!;
        
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
    static string ICommand.Name => ""{commandName}"";
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

internal static partial class {commandManagerTypeName}
{{
    private const int MAX_COMMAND_LENGTH = 4096;

{string.Join("\n\n", commands
    .Select(c => $"    private static ReadOnlySpan<byte> {GetCommandSpanName(c)} => new byte[] {{ {GetCommandSpanBytes(c)} }};"))}

    public static CommandExecutionResult ExecuteCommand(
        Stream source,
        TextWriter writer,
        EternalFileSystem fileSystem,
        List<string> currentDirectory)
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

        CommandExecutionContext context = new(fileSystem, buffer[(spaceIndex + 1)..], writer, currentDirectory);

        return commandSpan switch
        {{
{string.Join("\n", commands.Select(SwitchCommand))}
            _ => HandleDefault(Encoding.UTF8.GetString(commandSpan))
        }};

        CommandExecutionResult HandleDefault(string command)
        {{
            writer.WriteLine($@""Unable to process """"{{command}}"""": command not found."");
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

            return $@"            {GetCommandSwitchCase(command)} => {commandDeclarationName}.Instance.Execute(ref context),";
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

    private static string GenerateCommandManagerDocumentation(ImmutableArray<INamedTypeSymbol> commands, string commandManagerTypeName)
    {
        IList<string> usings = new HashSet<string>(CollectUsings(commands))
        {
            typeof(CommandAttribute).Namespace,
            typeof(List<>).Namespace
        }.OrderUsings();

        return $@"
{string.Join("\n", usings.Select(u => $"using {u};"))}

#nullable enable

internal static partial class {commandManagerTypeName}
{{
    public static readonly Dictionary<string, CommandInfo?> Commands = new()
    {{
        {string.Join(",\n", commands
            .Select(c => $@"{{ ""{GetCommandName(c)}"", {GetCommandInfo(c)} }}"))}
    }};
}}";
        static string GetCommandInfo(INamedTypeSymbol command)
        {
            var commandDocAttribute = command.GetAttribute<CommandDocAttribute>();

            if (commandDocAttribute is null)
                return "null";

            var commandSummary = (string)commandDocAttribute.ConstructorArguments[0].Value!;

            return $@"new CommandInfo(""{commandSummary}"")";
        }
    }

    private static string GetCommandName(INamedTypeSymbol command)
    {
        var commandAttribute = command.GetAttribute<CommandAttribute>()!;
        var commandName = (string)commandAttribute.ConstructorArguments[0].Value!;
        return commandName;
    }
}
