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
    
    private Expr FinishCall(Expr callee)
    {
        var arguments = new List<Expr>();
        
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                arguments.Add(Expression());
            }
            while (Match(TokenType.COMMA));
        }
        
        var paren = Consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments");
        
        return new Expr.Call
        {
            Callee = callee,
            Paren = paren,
            Arguments = arguments
        };
    }
    
    private Expr Call()
    {
        var expr = Primary();
        
        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else
            {
                break;
            }
        }
        
        return expr;
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
        
        return Call();
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
    
    private Expr And()
    {
        var expr = Equality();
        
        while (Match(TokenType.AND))
        {
            expr = new Expr.Logical
            {
                Left = expr,
                Operator = Previous(),
                Right = Equality()
            };
        }
        
        return expr;
    }
    
    private Expr Or()
    {
        var expr = And();
        
        while (Match(TokenType.OR))
        {
            expr = new Expr.Logical
            {
                Left = expr,
                Operator = Previous(),
                Right = And()
            };
        }
        
        return expr;
    }
    
    private Expr Assignment()
    {
        var expr = Or();
        
        if (Match(TokenType.EQUAL))
        {
            var equals = Previous();
            var value = Assignment();
            
            if (expr is Expr.Variable variable)
            {
                return new Expr.Assign
                {
                    Name = variable.Name,
                    Value = value
                };
            }
            
            throw new SyntaxException("Invalid assignment target", equals.Line);
        }
        
        return expr;
    }
    
    private Expr Expression()
    {
        return Assignment();
    }

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
        
        var body = Statement();
        
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
            Body = body
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
            ElseBranch = elseBranch
        };
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

    private Stmt ReturnStatement()
    {
        var keyword = Previous();
        Expr? value = null;
        
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }
        
        Consume(TokenType.SEMICOLON, "Expected ';' after return value");
        return new Stmt.Return
        {
            Keyword = keyword,
            Value = value
        };
    }
    
    private Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after 'while'");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after condition");
        
        var body = Statement();
        
        return new Stmt.While
        {
            Condition = condition,
            Body = body
        };
    }
    
    private IList<Stmt> Block()
    {
        var statements = new List<Stmt>();
        
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
        {
            statements.Add(Declaration());
        }
        
        Consume(TokenType.RIGHT_BRACE, "Expected '}' after block");
        return statements;
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
        if (Match(TokenType.FOR))
        {
            return ForStatement();
        }
        
        if (Match(TokenType.IF))
        {
            return IfStatement();
        }
        
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }
        
        if (Match(TokenType.RETURN))
        {
            return ReturnStatement();
        }

        if (Match(TokenType.WHILE))
        {
            return WhileStatement();
        }
        
        if (Match(TokenType.LEFT_BRACE))
        {
            return new Stmt.Block
            {
                Statements = Block()
            };
        }
        
        return ExpressionStatement();
    }

    private Stmt FuncDeclaration()
    {
        bool ticking = false;
        bool initializing = false;
        while (Match(TokenType.TICKING, TokenType.INITIALIZING))
        {
            if (Previous().Type == TokenType.TICKING)
            {
                ticking = true;
            }

            if (Previous().Type == TokenType.INITIALIZING)
            {
                initializing = true;
            }
        }
        
        Consume(TokenType.FUNCTION, "Expected 'function' after 'ticking' or 'initializing'");
        
        var name = Consume(TokenType.IDENTIFIER, "Expected function name");
        
        Consume(TokenType.LEFT_PAREN, "Expected '(' after function name");
        
        var parameters = new List<Token>();
        
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expected parameter name"));
            }
            while (Match(TokenType.COMMA));
        }
        
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after parameters");
        
        Consume(TokenType.LEFT_BRACE, "Expected '{' before function body");
        
        var body = Block();
        
        return new Stmt.Function
        {
            Name = name,
            Params = parameters,
            Body = body,
            Ticking = ticking,
            Initializing = initializing
        };
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
        if (Match(TokenType.TICKING, TokenType.INITIALIZING, TokenType.FUNCTION))
        {
            return FuncDeclaration();
        }
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