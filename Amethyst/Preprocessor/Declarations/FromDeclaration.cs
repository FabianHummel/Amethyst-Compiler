using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Imports symbols from another resource. Symbols are automatically exported when declared at
    /// the top-level scope.<br /><inheritdoc /></summary>
    /// <example><c>FROM `my_namespace:path/to/resource` IMPORT symbol1, symbol2, symbol3</c></example>
    /// <exception cref="SemanticException">The specified resource does not export one or more of the
    /// requested symbols.</exception>
    public override object? VisitPreprocessorFromDeclaration(AmethystParser.PreprocessorFromDeclarationContext context)
    {
        // var resourcePath = VisitResourceLiteral(context.resourceLiteral());
        var resourcePath = context.RESOURCE_LITERAL().GetText()[1..^1];
        var resource = GetSourceFile(resourcePath, Constants.DatapackFunctionsDirectory, context);

        var symbols = context.IDENTIFIER()
            .Select(s => s.GetText())
            .ToList();
        
        foreach (var symbol in symbols)
        {
            if (!resource.ExportedSymbols.ContainsKey(symbol))
            {
                throw new SemanticException($"'{resource.Name}' does not export symbol '{symbol}'", context);
            }
        }

        return null;
    }
}