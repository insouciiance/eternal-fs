using System.Collections.Generic;

namespace EternalFS.Library.Commands.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public TextSpan Location { get; }

    public SyntaxTree SyntaxTree { get; }

    protected SyntaxNode(TextSpan location, SyntaxTree tree)
    {
        Location = location;
        SyntaxTree = tree;
    }

    public abstract IEnumerable<SyntaxNode> DesdendantNodes(bool includeSelf);

    public override string ToString()
    {
        return SyntaxTree.ToString()[Location.Start..(Location.End + 1)];
    }
}
