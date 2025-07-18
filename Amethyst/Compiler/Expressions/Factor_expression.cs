using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitFactor_expression(AmethystParser.Factor_expressionContext context)
    {
        if (context.unary_expression() is not { } unaryExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (unaryExpressionContexts.Length == 1)
        {
            return VisitUnary_expression(unaryExpressionContexts[0]);
        }
        
        if (VisitUnary_expression(unaryExpressionContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected unary expression.", unaryExpressionContexts[0]);
        }

        for (var i = 1; i < unaryExpressionContexts.Length; i++)
        {
            if (VisitUnary_expression(unaryExpressionContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected unary expression.", unaryExpressionContexts[i]);
            }
            
            var operatorToken = context.GetChild(2 * i - 1).GetText();

            if (Enum.GetValues<ArithmeticOperator>().First(op => op.GetAmethystOperatorSymbol() == operatorToken) is var op)
            {
                if (previous.TryCalculateConstants(current, op, out var result))
                {
                    previous = result;
                    continue;
                }
                
                var lhs = previous.ToRuntimeValue();
                var rhs = current.ToRuntimeValue();

                previous = op switch
                {
                    ArithmeticOperator.MULTIPLY => lhs * rhs,
                    ArithmeticOperator.DIVIDE => lhs / rhs,
                    ArithmeticOperator.MODULO => lhs % rhs,
                    _ => throw new SyntaxException("Expected operator.", context)
                };
            }
        }

        return previous;
    }
}