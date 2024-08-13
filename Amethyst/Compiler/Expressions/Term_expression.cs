using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitTerm_expression(AmethystParser.Term_expressionContext context)
    {
        if (context.factor_expression() is not { } factorExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (factorExpressionContexts.Length == 1)
        {
            return VisitFactor_expression(factorExpressionContexts[0]);
        }
        
        if (VisitFactor_expression(factorExpressionContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected factor expression.", factorExpressionContexts[0]);
        }

        for (var i = 1; i < factorExpressionContexts.Length; i++)
        {
            if (VisitFactor_expression(factorExpressionContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected factor expression.", factorExpressionContexts[i]);
            }

            var operatorToken = context.GetChild(2 * i - 1).GetText();

            if (operatorToken == "-")
            {
                previous -= current;
            }
            else if (operatorToken == "+")
            {
                previous += current;
            }
            else
            {
                throw new SyntaxException("Expected operator.", context);
            }
        }

        return previous;
    }
}