using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr And()
    {
        var expr = Equality();

        while (Match(TokenType.AND))
        {
            expr = new Expr.Logical
            {
                Left = expr,
                Operator = Previous(),
                Right = Equality()
            };
        }

        return expr;
    }
}