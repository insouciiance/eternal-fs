using System;
using System.Collections.Generic;

namespace EternalFS.Library.Commands.Syntax;

public sealed class ArgumentNode : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.Argument;

    public ArgumentNameNode Name { get; }

    public ArgumentValueNode? Value { get; }

    public ArgumentNode(TextSpan location, SyntaxTree tree, ArgumentNameNode name, ArgumentValueNode? value)
        : base(location, tree)
    {
        Name = name;
        Value = value;
    }

    public override IEnumerable<SyntaxNode> DesdendantNodes(bool includeSelf)
    {
        yield return Name;

        if (Value is not null)
            yield return Value;
    }
}
