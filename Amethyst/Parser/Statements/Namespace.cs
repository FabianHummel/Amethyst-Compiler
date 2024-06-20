using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt NsDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expected namespace name");
        Consume(TokenType.LEFT_BRACE, "Expected '{' before namespace body");

        var body = Declaration();

        return new Stmt.Namespace
        {
            Name = name,
            Body = body,
        };
    }
}