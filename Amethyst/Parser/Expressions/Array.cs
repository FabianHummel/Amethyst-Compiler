using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Array()
    {
        IList<Expr> values = new List<Expr>();

        if (!Check(TokenType.RIGHT_BRACKET))
        {
            do
            {
                values.Add(Expression());
            } while (Match(TokenType.COMMA));
        }

        Consume(TokenType.RIGHT_BRACKET, "Expected ']' after array");

        return new Expr.Array
        {
            Values = values
        };
    }
}