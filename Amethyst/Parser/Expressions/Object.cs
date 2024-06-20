using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private KeyValuePair<Expr, Expr> Mapping()
    {
        var key = Expression();
        Consume(TokenType.COLON, "Expected ':' after key");
        var value = Expression();
        return new KeyValuePair<Expr, Expr>(key, value);
    }

    private Expr Object()
    {
        IDictionary<Expr, Expr> mappings = new Dictionary<Expr, Expr>();

        if (!Check(TokenType.RIGHT_BRACE))
        {
            do
            {
                mappings.Add(Mapping());
            } while (Match(TokenType.COMMA));
        }

        Consume(TokenType.RIGHT_BRACE, "Expected '}' after object");

        return new Expr.Object
        {
            Values = mappings
        };
    }
}