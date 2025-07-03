using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitComparison_expression(AmethystParser.Comparison_expressionContext context)
    {
        if (context.term_expression() is not { } termExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (termExpressionContexts.Length == 1)
        {
            return VisitTerm_expression(termExpressionContexts[0]);
        }
        
        if (VisitTerm_expression(termExpressionContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected term expression.", termExpressionContexts[0]);
        }
        
        for (var i = 1; i < context.term_expression().Length; i++)
        {
            if (VisitTerm_expression(termExpressionContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected term expression.", termExpressionContexts[i]);
            }
            
            var operatorToken = context.GetChild(2 * i - 1).GetText();
            
            if (Enum.GetValues<ComparisonOperator>().First(op => op.GetAmethystOperatorSymbol() == operatorToken) is var op)
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
                    ComparisonOperator.LESS_THAN => lhs < rhs,
                    ComparisonOperator.LESS_THAN_OR_EQUAL => lhs <= rhs,
                    ComparisonOperator.GREATER_THAN => lhs > rhs,
                    ComparisonOperator.GREATER_THAN_OR_EQUAL => lhs >= rhs,
                    _ => throw new SyntaxException("Expected operator.", context)
                };
            }
        }

        return previous;
    }
}