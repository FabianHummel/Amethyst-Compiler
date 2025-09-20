using Amethyst.Language;

namespace Amethyst;

public interface IIndexable
{
    AbstractResult GetIndex(AbstractResult index);
}

public partial class Compiler
{
    public override AbstractResult VisitIndexExpression(AmethystParser.IndexExpressionContext context)
    {
        var expressionContexts = context.expression();
        var result = VisitExpression(expressionContexts[0]);
        var index = VisitExpression(expressionContexts[1]);

        if (result is not IIndexable indexable)
        {
            throw new SyntaxException($"Type '{result.DataType}' is not indexable.", context);
        }

        return indexable.GetIndex(index);
    }
}