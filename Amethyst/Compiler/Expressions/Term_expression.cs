using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

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
        
        if (VisitFactor_expression(factorExpressionContexts[0]) is not { DataType.IsScoreboardType: true } previous)
        {
            throw new SyntaxException("Expected factor expression.", factorExpressionContexts[0]);
        }

        for (var i = 1; i < factorExpressionContexts.Length; i++)
        {
            if (VisitFactor_expression(factorExpressionContexts[i]) is not { DataType.IsScoreboardType: true } current)
            {
                throw new SyntaxException("Expected factor expression.", factorExpressionContexts[i]);
            }

            var operatorToken = context.GetChild(2 * i - 1).GetText();

            if (operatorToken == "-")
            {
                previous = Visit_subtract(previous, current, factorExpressionContexts[i]);
            }
            else if (operatorToken == "+")
            {
                previous = Visit_add(previous, current, factorExpressionContexts[i]);
            }
            else
            {
                throw new SyntaxException("Expected operator.", context);
            }
        }

        return previous;
    }
}