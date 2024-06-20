using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Comparison()
            };
        }

        return expr;
    }
}