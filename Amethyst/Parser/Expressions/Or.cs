using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Or()
    {
        var expr = And();

        while (Match(TokenType.OR))
        {
            expr = new Expr.Logical
            {
                Left = expr,
                Operator = Previous(),
                Right = And()
            };
        }

        return expr;
    }
}