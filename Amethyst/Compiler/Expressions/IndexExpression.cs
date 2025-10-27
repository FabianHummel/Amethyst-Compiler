using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitIndexExpression(AmethystParser.IndexExpressionContext context)
    {
        var expressionContexts = context.expression();
        var result = VisitExpression(expressionContexts[0]);
        var index = VisitExpression(expressionContexts[1]);

        if (result is not IIndexable indexable)
        {
            throw new SyntaxException($"Type '{result.Datatype}' is not indexable.", context);
        }

        return indexable.GetIndex(index);
    }
}