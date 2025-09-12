using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitComparisonExpression(AmethystParser.ComparisonExpressionContext context)
    {
        var expressionContexts = context.expression();
        var left = VisitExpression(expressionContexts[0]);
        var right = VisitExpression(expressionContexts[1]);
        var operatorToken = context.GetChild(1).GetText();
        
        var op = Enum.GetValues<ComparisonOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);
        
        if (left.TryCalculateConstants(right, op, out var result))
        {
            return result;
        }
                
        var lhs = left.ToRuntimeValue();
        var rhs = right.ToRuntimeValue();

        return op switch
        {
            ComparisonOperator.LESS_THAN => lhs < rhs,
            ComparisonOperator.LESS_THAN_OR_EQUAL => lhs <= rhs,
            ComparisonOperator.GREATER_THAN => lhs > rhs,
            ComparisonOperator.GREATER_THAN_OR_EQUAL => lhs >= rhs,
            _ => throw new SyntaxException("Expected operator.", context)
        };
    }
}