using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Comparison()
    {
        var expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Term()
            };
        }

        return expr;
    }
}