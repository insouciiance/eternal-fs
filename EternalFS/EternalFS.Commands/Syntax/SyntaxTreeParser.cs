using EternalFS.Commands.Syntax.NodeBuilders;

namespace EternalFS.Commands.Syntax;

internal static class SyntaxTreeParser
{
    public static SyntaxTree ParseText(string fullString)
    {
        SyntaxTree tree = new(fullString);

        // assume CommandDeclarationNode is always the root
        CommandDeclarationNodeBuilder builder = new(tree);
        CommandDeclarationNode root = builder.ParseNode(fullString, new(0, fullString.Length - 1));

        tree.Root = root;

        return tree;
    }
}
