using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt VarDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expected variable name");

        Consume(TokenType.EQUAL, "Expected '=' after variable name");

        Expr? initializer = null;
        if (!Check(TokenType.SEMICOLON))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration");
        return new Stmt.Var
        {
            Name = name,
            Initializer = initializer,
        };
    }
}