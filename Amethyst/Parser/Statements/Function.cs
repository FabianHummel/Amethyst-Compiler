using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt FuncDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expected function name");

        Consume(TokenType.LEFT_PAREN, "Expected '(' after function name");

        var parameters = new List<Token>();

        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expected parameter name"));
            } while (Match(TokenType.COMMA));
        }

        Consume(TokenType.RIGHT_PAREN, "Expected ')' after parameters");

        Consume(TokenType.LEFT_BRACE, "Expected '{' before function body");

        var body = Block();

        return new Stmt.Function
        {
            Name = name,
            Params = parameters,
            Body = body,
        };
    }
}