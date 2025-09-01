using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFrom(AmethystParser.FromContext context)
    {
        if (context.RESOURCE_LITERAL() is not { } resourcePath)
        {
            throw new SyntaxException("Expected resource path for the import path.", context);
        }

        var resource = VisitResource(resourcePath.GetText(), Constants.DATAPACK_FUNCTIONS_DIRECTORY);

        var symbols = context.IDENTIFIER()
            .Select(s => s.GetText())
            .ToList();

        if (symbols.Count == 0)
        {
            throw new SyntaxException("Expected at least one symbol to import.", context);
        }
        
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