using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Term()
    {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Factor()
            };
        }

        return expr;
    }
}