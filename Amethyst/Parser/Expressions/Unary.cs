using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            return new Expr.Unary
            {
                Operator = Previous(),
                Right = Unary()
            };
        }

        return Call();
    }
}