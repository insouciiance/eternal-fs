namespace EternalFS.Commands.Syntax.NodeBuilders;

internal class ArgumentNameNodeBuilder : INodeBuilder<ArgumentNameNode>
{
    public SyntaxTree SyntaxTree { get; }

    public ArgumentNameNodeBuilder(SyntaxTree tree)
    {
        SyntaxTree = tree;
    }

    public ArgumentNameNode ParseNode(string sourceText, TextSpan location)
    {
        return new ArgumentNameNode(location, SyntaxTree);
    }
}
