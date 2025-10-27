using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitExpressionSelector(AmethystParser.ExpressionSelectorContext context)
    {
        var identifier = context.IDENTIFIER();
        var queryKey = identifier.GetText();
            
        var expressionContext = context.expression();

        if (queryKey is "x" or "y" or "z" or "dx" or "dy" or "dz" or "distance" or "x_rotation" or "y_rotation")
        {
            return VisitNumericSelector(queryKey, expressionContext, allowDecimals: true);
        }

        if (queryKey is "tag")
        {
            return VisitStringSelector(queryKey, expressionContext);
        }

        if (queryKey is "tags")
        {
            return VisitTagsSelector(expressionContext);
        }
        
        throw new SyntaxException($"Invalid query key '{queryKey}' for expression selector.", context);
    }
}