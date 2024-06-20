using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after 'for'");

        Stmt? initializer;
        if (Match(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(TokenType.VAR))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expr? condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expected ';' after loop condition");

        Expr? increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }

        Consume(TokenType.RIGHT_PAREN, "Expected ')' after for clauses");

        var body = Block();

        if (increment != null)
        {
            body = new Stmt.Block
            {
                Statements = new List<Stmt>
                {
                    body,
                    new Stmt.Expression { Expr = increment }
                }
            };
        }

        if (condition == null)
        {
            condition = new Expr.Literal { Value = true };
        }

        body = new Stmt.While
        {
            Condition = condition,
            Body = body,
        };

        if (initializer != null)
        {
            body = new Stmt.Block
            {
                Statements = new List<Stmt>
                {
                    initializer,
                    body
                }
            };
        }

        return body;
    }
}