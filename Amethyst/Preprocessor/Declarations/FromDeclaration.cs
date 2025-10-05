using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorFromDeclaration(AmethystParser.PreprocessorFromDeclarationContext context)
    {
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
}