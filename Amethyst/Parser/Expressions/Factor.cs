using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Unary()
            };
        }

        return expr;
    }
}