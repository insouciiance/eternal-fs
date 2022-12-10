using System;

namespace EternalFS.Library.Commands.Syntax.NodeBuilders;

internal interface INodeBuilder<T>
    where T : SyntaxNode
{
    SyntaxTree SyntaxTree { get; }

    T ParseNode(string sourceText, TextSpan location);
}
