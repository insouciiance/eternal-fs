namespace EternalFS.Commands.Syntax.NodeBuilders;

internal class CommandNameNodeBuilder : INodeBuilder<CommandNameNode>
{
    public SyntaxTree SyntaxTree { get; }

    public CommandNameNodeBuilder(SyntaxTree tree)
    {
        SyntaxTree = tree;
    }

    public CommandNameNode ParseNode(string sourceText, TextSpan location)
    {
        return new CommandNameNode(location, SyntaxTree);
    }
}
