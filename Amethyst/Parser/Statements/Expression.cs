using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt ExpressionStatement()
    {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after expression");
        return new Stmt.Expression
        {
            Expr = value
        };
    }
}