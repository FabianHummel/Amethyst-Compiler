using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Accesses an index of any value that is indexable (implementing <see cref="IIndexable" />).</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">The target is not indexable.</exception>
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