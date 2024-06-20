using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt DebugStatement()
    {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after value");
        return new Stmt.Debug
        {
            Expr = value,
        };
    }
}