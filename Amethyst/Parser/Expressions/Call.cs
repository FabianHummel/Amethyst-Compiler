using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Call()
    {
        var expr = Primary();

        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else
            {
                break;
            }
        }

        return expr;
    }
    
    private Expr FinishCall(Expr callee)
    {
        var arguments = new List<Expr>();

        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }

        Consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments");

        return new Expr.Call
        {
            Callee = callee,
            Arguments = arguments
        };
    }
}