using System;
using System.Collections.Generic;

namespace EternalFS.Library.Commands.Syntax;

public sealed class CommandNameNode : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.CommandName;

    public string Identifier { get; }

    public CommandNameNode(TextSpan location, SyntaxTree tree)
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
