using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt ContinueStatement()
    {
        Consume(TokenType.SEMICOLON, "Expected ';' after continue");
        return new Stmt.Continue
        {
        };
    }
}