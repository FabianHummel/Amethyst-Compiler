using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt ReturnStatement()
    {
        Expr? value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expected ';' after return value");
        return new Stmt.Return
        {
            Value = value,
        };
    }
}