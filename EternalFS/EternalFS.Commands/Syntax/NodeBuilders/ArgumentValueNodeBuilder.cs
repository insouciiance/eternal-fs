namespace EternalFS.Commands.Syntax.NodeBuilders;

internal class ArgumentValueNodeBuilder : INodeBuilder<ArgumentValueNode>
{
    public SyntaxTree SyntaxTree { get; }

    public ArgumentValueNodeBuilder(SyntaxTree tree)
    {
        SyntaxTree = tree;
    }

    public ArgumentValueNode ParseNode(string sourceText, TextSpan location)
    {
        return new ArgumentValueNode(location, SyntaxTree);
    }
}
