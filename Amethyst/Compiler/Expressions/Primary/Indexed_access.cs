using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitIndexed_access(AmethystParser.Indexed_accessContext context)
    {
        if (context.primary_expression() is not { } primaryExpressionContext)
        {
            throw new SyntaxException("Expected primary expression.", context);
        }

        var result = VisitPrimary_expression(primaryExpressionContext);

        if (context.expression() is not { } expressionContext)
        {
            throw new SyntaxException("Expected index expression.", context);
        }

        var index = VisitExpression(expressionContext);

        if (result is not IIndexable indexable)
        {
            throw new SyntaxException($"Type '{result.DataType}' is not indexable.", primaryExpressionContext);
        }

        return indexable.GetIndex(index);
    }
}