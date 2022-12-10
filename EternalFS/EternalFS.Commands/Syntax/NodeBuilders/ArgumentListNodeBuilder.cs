using System.Collections.Immutable;

namespace EternalFS.Commands.Syntax.NodeBuilders;

internal class ArgumentListNodeBuilder : INodeBuilder<ArgumentListNode>
{
    public SyntaxTree SyntaxTree { get; }

    public ArgumentListNodeBuilder(SyntaxTree tree)
    {
        SyntaxTree = tree;
    }

    public ArgumentListNode ParseNode(string sourceText, TextSpan location)
    {
        int start = location.Start;
        int end = start;

        var argumentsBuilder = ImmutableArray.CreateBuilder<ArgumentNode>();

        while (end <= location.End)
        {
            if (sourceText[end] is not ' ')
            {
                end++;
                continue;
            }

            ParseArgumentNode();

            end++;
            start = end;
        }

        ParseArgumentNode();

        return new ArgumentListNode(argumentsBuilder.ToImmutable(), location, SyntaxTree);
    
        void ParseArgumentNode()
        {
            TextSpan argumentLocation = new(start, end - 1);
            ArgumentNodeBuilder builder = new(SyntaxTree);
            argumentsBuilder.Add(builder.ParseNode(sourceText, argumentLocation));
        }
    }
}
