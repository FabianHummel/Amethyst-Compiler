using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt CommentStatement()
    {
        var value = Consume(TokenType.STRING, "Expected string after comment");
        Consume(TokenType.SEMICOLON, "Expected ';' after comment");
        return new Stmt.Comment
        {
            Value = value
        };
    }
}