using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitTerm(AmethystParser.TermContext context)
    {
        if (context.factor() is not { } factorContexts)
        {
            throw new UnreachableException();
        }
        
        if (factorContexts.Length == 1)
        {
            return VisitFactor(factorContexts[0]);
        }
        
        if (VisitFactor(factorContexts[0]) is not { DataType.IsScoreboardType: true } previous)
        {
            throw new SyntaxException("Expected factor expression.", factorContexts[0]);
        }

        for (var i = 1; i < factorContexts.Length; i++)
        {
            if (VisitFactor(factorContexts[i]) is not { DataType.IsScoreboardType: true } current)
            {
                throw new SyntaxException("Expected factor expression.", factorContexts[i]);
            }

            var operatorToken = context.GetChild(2 * i - 1).GetText();

            if (operatorToken == "-")
            {
                previous = Visit_subtract(previous, current, factorContexts[i]);
            }
            else if (operatorToken == "+")
            {
                previous = Visit_add(previous, current, factorContexts[i]);
            }
            else
            {
                throw new SyntaxException("Expected operator.", context);
            }
        }

        return previous;
    }
}