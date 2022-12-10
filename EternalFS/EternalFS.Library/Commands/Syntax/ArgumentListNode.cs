using System.Collections.Generic;
using System.Collections.Immutable;

namespace EternalFS.Library.Commands.Syntax;

public class ArgumentListNode : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.ArgumentList;

    public ImmutableArray<ArgumentNode> Arguments { get; }

    public ArgumentListNode(ImmutableArray<ArgumentNode> arguments, TextSpan location, SyntaxTree tree)
        : base(location, tree)
    {
        Arguments = arguments;
    }

    public override IEnumerable<SyntaxNode> DesdendantNodes(bool includeSelf)
    {
        if (includeSelf)
            yield return this;

        foreach (var argument in Arguments)
        {
            foreach (var descendant in argument.DesdendantNodes(true))
                yield return descendant;
        }
    }
}
