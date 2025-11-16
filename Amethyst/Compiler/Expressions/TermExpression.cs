using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Evaluates a term of two values. This can be either an addition or a subtraction operation.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">The operator is invalid.</exception>
    public override AbstractValue VisitTermExpression(AmethystParser.TermExpressionContext context)
    {
        var expressionContexts = context.expression();
        var left = VisitExpression(expressionContexts[0]);
        var right = VisitExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ArithmeticOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);
        
        return OperationRegistry.Resolve<AbstractValue, ArithmeticOperator>(this, context, op, left, right);
    }
}