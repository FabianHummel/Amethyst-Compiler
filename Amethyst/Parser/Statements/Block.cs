using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
        {
            statements.Add(Statement());
        }

        Consume(TokenType.RIGHT_BRACE, "Expected '}' after block");
        return new Stmt.Block
        {
            Statements = statements
        };
    }
}