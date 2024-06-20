using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after 'while'");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after condition");

        var body = Statement();

        return new Stmt.While
        {
            Condition = condition,
            Body = body,
        };
    }
}