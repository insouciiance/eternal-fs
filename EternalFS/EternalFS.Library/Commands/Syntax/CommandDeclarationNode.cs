using System.Collections.Generic;

namespace EternalFS.Library.Commands.Syntax;

public class CommandDeclarationNode : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.CommandDeclaration;

    public CommandNameNode CommandName { get; }

    public ArgumentListNode Arguments { get; }

    public CommandDeclarationNode(CommandNameNode commandName, ArgumentListNode arguments, TextSpan location, SyntaxTree root)
        : base(location, root)
    {
        CommandName = commandName;
        Arguments = arguments;
    }

    public override IEnumerable<SyntaxNode> DesdendantNodes(bool includeSelf)
    {
        if (includeSelf)
            yield return this;

        foreach (var descendant in CommandName.DesdendantNodes(true))
            yield return descendant;

        foreach (var argument in Arguments.DesdendantNodes(true))
            yield return argument;
    }
}
