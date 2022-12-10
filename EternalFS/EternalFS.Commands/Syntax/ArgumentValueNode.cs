using System;
using System.Collections.Generic;

namespace EternalFS.Commands.Syntax;

public sealed class ArgumentValueNode : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.ArgumentValue;

    public string Value { get; set; }

    public ArgumentValueNode(TextSpan location, SyntaxTree tree)
        : base(location, tree)
    {
        Value = tree.SourceText[location.Start..(location.End + 1)];
    }

    public override IEnumerable<SyntaxNode> DesdendantNodes(bool includeSelf)
    {
        if (includeSelf)
            yield return this;
    }
}
