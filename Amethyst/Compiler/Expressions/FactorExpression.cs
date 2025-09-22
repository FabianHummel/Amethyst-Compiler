using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitFactorExpression(AmethystParser.FactorExpressionContext context)
    {
        var expressionContexts = context.expression();
        var left = VisitExpression(expressionContexts[0]);
        var right = VisitExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ArithmeticOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);
        
        return op switch
        {
            ArithmeticOperator.MULTIPLY => left * right,
            ArithmeticOperator.DIVIDE => left / right,
            ArithmeticOperator.MODULO => left % right,
            _ => throw new SyntaxException("Expected operator.", context)
        };
    }
}