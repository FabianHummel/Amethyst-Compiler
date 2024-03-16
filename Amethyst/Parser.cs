namespace Amethyst;

public class Parser
{
    private IList<Token> Tokens { get; }
    private int current;
    
    public Parser(IList<Token> tokens)
    {
        Tokens = tokens;
    }
    
    private Token Previous()
    {
        return Tokens[current - 1];
    }
    
    private Token Peek()
    {
        return Tokens[current];
    }

    private bool IsAtEnd => Peek().Type == TokenType.EOF;
    
    private Token Advance()
    {
        if (!IsAtEnd)
        {
            current++;
        }
        
        return Previous();
    }
    
    private bool Check(TokenType type)
    {
        if (IsAtEnd)
        {
            return false;
        }
        
        return Peek().Type == type;
    }
    
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        
        return false;
    }
    
    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        
        throw new SyntaxException(message, Peek().Line);
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
        {
            return new Expr.Literal { Value = false };
        }
        
        if (Match(TokenType.TRUE))
        {
            return new Expr.Literal { Value = true };
        }
        
        if (Match(TokenType.NULL))
        {
            return new Expr.Literal { Value = null };
        }
        
        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal { Value = Previous().Literal };
        }
        
        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable { Name = Previous() };
        }
        
        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
            return new Expr.Grouping { Expression = expr };
        }

        throw new SyntaxException("Expected expression", Peek().Line);
    }
    
    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            return new Expr.Unary
            {
                Operator = Previous(),
                Right = Unary()
            };
        }
        
        return Primary();
    }
    
    private Expr Factor()
    {
        var expr = Unary();
        
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Unary()
            };
        }
        
        return expr;
    }
    
    private Expr Term()
    {
        var expr = Factor();
        
        while (Match(TokenType.MINUS, TokenType.PLUS))
        { 
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Factor()
            };
        }
        
        return expr;
    }
    
    private Expr Comparison()
    {
        var expr = Term();
        
        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Term()
            };
        }
        
        return expr;
    }
    
    private Expr Equality()
    {
        var expr = Comparison();
        
        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = Previous(),
                Right = Comparison()
            };
        }
        
        return expr;
    }
    
    private Expr Expression()
    {
        return Equality();
    }
    
    private Stmt PrintStatement()
    {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after value");
        return new Stmt.Print
        { 
            Expr = value
        };
    }
    
    private Stmt ExpressionStatement()
    {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after expression");
        return new Stmt.Expression
        {
            Expr = value
        };
    }
    
    private Stmt Statement()
    {
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }
        
        return ExpressionStatement();
    }
    
    private Stmt VarDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expected variable name");
        
        Expr? initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Expression();
        }
        
        Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration");
        return new Stmt.Var
        {
            Name = name,
            Initializer = initializer
        };
    }

    private Stmt Declaration()
    {
        if (Match(TokenType.VAR))
        {
            return VarDeclaration();
        }
        
        return Statement();
    }

    public IList<Stmt> Parse()
    {
        var statements = new List<Stmt>();
        
        while (!IsAtEnd)
        {
            statements.Add(Declaration());
        }
        
        return statements;
    }
}