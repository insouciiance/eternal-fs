namespace EternalFS.Library.Commands.Syntax.NodeBuilders;

internal class ArgumentNodeBuilder : INodeBuilder<ArgumentNode>
{
    private const string EQUALS_SEPARATOR = "=";

    public SyntaxTree SyntaxTree { get; }

    public ArgumentNodeBuilder(SyntaxTree tree)
    {
        SyntaxTree = tree;
    }
    
    public ArgumentNode ParseNode(string sourceText, TextSpan location)
    {
        int equalsIndex = sourceText[location.Start..location.End].IndexOf(EQUALS_SEPARATOR);

        ArgumentNameNodeBuilder nameNodeBuilder = new(SyntaxTree);

        ArgumentNameNode nameNode;
        ArgumentValueNode? valueNode = null;

        if (equalsIndex != -1)
        {
            var nameLocation = new TextSpan(location.Start, location.Start + equalsIndex - 1);
            var valueLocation = new TextSpan(location.Start + equalsIndex + 1, location.End);

            nameNode = nameNodeBuilder.ParseNode(sourceText, nameLocation);

            ArgumentValueNodeBuilder valueNodeBuilder = new(SyntaxTree);

            valueNode = valueNodeBuilder.ParseNode(sourceText, valueLocation);
        }
        else
        {
            nameNode = nameNodeBuilder.ParseNode(sourceText, location);
        }

        return new(location, SyntaxTree, nameNode, valueNode);
    }
}
