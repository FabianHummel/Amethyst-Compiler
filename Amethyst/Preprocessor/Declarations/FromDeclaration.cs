using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorFromImportDeclaration(AmethystParser.PreprocessorFromImportDeclarationContext context)
    {
        // var resourcePath = VisitResourceLiteral(context.resourceLiteral());
        var resourcePath = context.RESOURCE_LITERAL().GetText()[1..^1];
        var resource = VisitResource(resourcePath, Constants.DatapackFunctionsDirectory, context);

        var symbols = context.IDENTIFIER()
            .Select(s => s.GetText())
            .ToList();
        
        foreach (var symbol in symbols)
        {
            if (!resource.ExportedSymbols.ContainsKey(symbol))
            {
                throw new SyntaxException($"'{resource.Name}' does not export symbol '{symbol}'", context);
            }
        }

        return null;
    }

    public override object? VisitPreprocessorFromAsDeclaration(AmethystParser.PreprocessorFromAsDeclarationContext context)
    {
        // var resourcePath = VisitResourceLiteral(context.resourceLiteral());
        var resourcePath = context.RESOURCE_LITERAL().GetText()[1..^1];
        var resource = VisitResource(resourcePath, Constants.DatapackFunctionsDirectory, context);
        var symbol = context.IDENTIFIER().GetText();
        
        // TODO: Implement

        return null;
    }
}