namespace EternalFS.Library.Commands.Syntax;

public class SyntaxTree
{
    public SyntaxNode Root { get; internal set; } = null!;

    public string SourceText { get; }

    internal SyntaxTree(string sourceText)
    {
        SourceText = sourceText;
    }

    internal SyntaxTree(SyntaxNode root, string sourceText)
        : this(sourceText)
    {
        Root = root;
    }

    public static SyntaxTree ParseText(string fullString)
    {
        return SyntaxTreeParser.ParseText(fullString);
    }

    public override string ToString()
    {
        return SourceText;
    }
}
