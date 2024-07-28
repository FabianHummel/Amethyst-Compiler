using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitFactor(AmethystParser.FactorContext context)
    {
        if (context.unary() is not { } unaryContexts)
        {
            throw new UnreachableException();
        }
        
        if (unaryContexts.Length == 1)
        {
            return VisitUnary(unaryContexts[0]);
        }
        
        if (VisitUnary(unaryContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected factor expression.", unaryContexts[0]);
        }

        for (var i = 1; i < unaryContexts.Length; i++)
        {
            if (VisitUnary(unaryContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected factor expression.", unaryContexts[i]);
            }

            var operatorToken = context.GetChild(2 * i - 1).GetText();

            if (operatorToken == "*")
            {
                previous = Visit_multiply(previous, current, unaryContexts[i]);
            }
            else if (operatorToken == "/")
            {
                previous = Visit_divide(previous, current, unaryContexts[i]);
            }
            else if (operatorToken == "%")
            {
                previous = Visit_modulo(previous, current, unaryContexts[i]);
            }
            else
            {
                throw new SyntaxException("Expected operator.", context);
            }
        }

        return previous;
    }
}