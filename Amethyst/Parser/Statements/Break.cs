using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt BreakStatement()
    {
        Consume(TokenType.SEMICOLON, "Expected ';' after break");
        return new Stmt.Break
        {
        };
    }
}