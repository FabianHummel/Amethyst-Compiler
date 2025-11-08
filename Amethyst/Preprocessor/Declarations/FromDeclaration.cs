using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Imports symbols from another resource. Symbols are automatically exported when declared at
    ///     the top-level scope.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><c>FROM `my_namespace:path/to/resource` IMPORT symbol1, symbol2, symbol3;</c></example>
    /// <exception cref="SemanticException">The specified resource does not export one or more of the
    /// requested symbols.</exception>
    /// <seealso cref="VisitPreprocessorImportAsDeclaration" />
    public override object? VisitPreprocessorFromImportDeclaration(AmethystParser.PreprocessorFromImportDeclarationContext context)
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

    /// <inheritdoc />
    /// <summary>
    ///     <p>Imports a specific resource directly as a preprocessor variable. This is useful for using
    ///     individual resources such as predicates, advancements or item models.</p>
    ///     <p><inheritdoc /></p>
    /// </summary>
    /// <example><c>IMPORT `my_namespace:path/to/resource` AS my_resource;</c></example>
    /// <seealso cref="VisitPreprocessorFromImportDeclaration" />
    public override object? VisitPreprocessorImportAsDeclaration(AmethystParser.PreprocessorImportAsDeclarationContext context)
    {
        // var resourcePath = VisitResourceLiteral(context.resourceLiteral());
        var resourcePath = context.RESOURCE_LITERAL().GetText()[1..^1];
        var resource = GetSourceFile(resourcePath, Constants.DatapackFunctionsDirectory, context);
        var symbol = context.IDENTIFIER().GetText();
        
        // TODO: Implement

        return null;
    }
}