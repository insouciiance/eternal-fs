using System;
using System.Collections.Generic;

namespace EternalFS.Library.Commands.Syntax;

public sealed class ArgumentNameNode : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.ArgumentName;

    public string Identifier { get; }

    public ArgumentNameNode(TextSpan location, SyntaxTree tree)
        : base(location, tree)
    {
        Identifier = tree.SourceText[location.Start..(location.End + 1)];
    }

    public override IEnumerable<SyntaxNode> DesdendantNodes(bool includeSelf)
    {
        if (includeSelf)
            yield return this;
    }
}
