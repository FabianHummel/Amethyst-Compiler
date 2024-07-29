using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

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
            throw new SyntaxException("Expected factor expression.", unaryExpressionContexts[0]);
        }

        for (var i = 1; i < unaryExpressionContexts.Length; i++)
        {
            if (VisitUnary_expression(unaryExpressionContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected factor expression.", unaryExpressionContexts[i]);
            }

            var operatorToken = context.GetChild(2 * i - 1).GetText();

            if (operatorToken == "*")
            {
                previous = Visit_multiply(previous, current, unaryExpressionContexts[i]);
            }
            else if (operatorToken == "/")
            {
                previous = Visit_divide(previous, current, unaryExpressionContexts[i]);
            }
            else if (operatorToken == "%")
            {
                previous = Visit_modulo(previous, current, unaryExpressionContexts[i]);
            }
            else
            {
                throw new SyntaxException("Expected operator.", context);
            }
        }

        return previous;
    }
}