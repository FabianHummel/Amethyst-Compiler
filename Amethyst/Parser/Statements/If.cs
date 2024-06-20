using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after 'if'");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after condition");

        var thenBranch = Statement();
        Stmt? elseBranch = null;

        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }

        return new Stmt.If
        {
            Condition = condition,
            ThenBranch = thenBranch,
            ElseBranch = elseBranch,
        };
    }
}