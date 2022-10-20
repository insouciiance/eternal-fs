using EternalFS.Generator.Templates;
using Microsoft.CodeAnalysis;

namespace EternalFS.Generator.Extensions;

public static class SourceProductionContextExtensions
{
    public static void AddFileSource(this SourceProductionContext context, string hintPath, string sourceText)
    {
        context.AddSource(hintPath, new FileTemplate(hintPath, sourceText).GetSource());
    }
}
