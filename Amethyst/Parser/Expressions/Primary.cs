using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Primary()
    {
        if (Match(TokenType.INTEGER, TokenType.DECIMAL, TokenType.STRING))
        {
            return new Expr.Literal { Value = Previous().Literal };
        }

        if (Match(TokenType.TRUE))
        {
            return new Expr.Literal { Value = true };
        }

        if (Match(TokenType.FALSE))
        {
            return new Expr.Literal { Value = false };
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
            return new Expr.Grouping { Expression = expr };
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable { Name = Previous() };
        }

        if (Match(TokenType.LEFT_BRACE))
        {
            return Object();
        }

        if (Match(TokenType.LEFT_BRACKET))
        {
            return Array();
        }

        throw new SyntaxException("Expected expression", Peek().Line, Context.SourcePath);
    }
}