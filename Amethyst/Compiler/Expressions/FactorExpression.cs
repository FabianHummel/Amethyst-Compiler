using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitFactorExpression(AmethystParser.FactorExpressionContext context)
    {
        var expressionContexts = context.expression();
        var left = VisitExpression(expressionContexts[0]);
        var right = VisitExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ArithmeticOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        if (left.TryCalculateConstants(right, op, out var result))
        {
            return result;
        }

        var lhs = left.ToRuntimeValue();
        var rhs = right.ToRuntimeValue();
        
        return op switch
        {
            ArithmeticOperator.MULTIPLY => lhs * rhs,
            ArithmeticOperator.DIVIDE => lhs / rhs,
            ArithmeticOperator.MODULO => lhs % rhs,
            _ => throw new SyntaxException("Expected operator.", context)
        };
    }
}