using System.Collections.Immutable;

namespace EternalFS.Library.Commands.Syntax.NodeBuilders;

internal class CommandDeclarationNodeBuilder : INodeBuilder<CommandDeclarationNode>
{
    private const string NAME_SEPARATOR = " ";

    public SyntaxTree SyntaxTree { get; }

    public CommandDeclarationNodeBuilder(SyntaxTree tree)
    {
        SyntaxTree = tree;
    }

    public CommandDeclarationNode ParseNode(string sourceText, TextSpan location)
    {
        int separatorIndex = sourceText[location.Start..location.End].IndexOf(NAME_SEPARATOR);

        CommandNameNode nameNode;
        ArgumentListNode argumentsNode;

        CommandNameNodeBuilder nameNodeBuilder = new(SyntaxTree);

        if (separatorIndex != -1)
        {
            TextSpan nameLocation = new(location.Start, location.Start + separatorIndex - 1);
            nameNode = nameNodeBuilder.ParseNode(sourceText, nameLocation);

            ArgumentListNodeBuilder argumentsNodeBuilder = new(SyntaxTree);
            TextSpan argumentsLocation = new(location.Start + separatorIndex + 1, location.End);
            argumentsNode = argumentsNodeBuilder.ParseNode(sourceText, argumentsLocation);
        }
        else
        {
            nameNode = nameNodeBuilder.ParseNode(sourceText, location);
            argumentsNode = new ArgumentListNode(ImmutableArray<ArgumentNode>.Empty, TextSpan.Empty, SyntaxTree);
        }

        return new(nameNode, argumentsNode, location, SyntaxTree);
    }
}
