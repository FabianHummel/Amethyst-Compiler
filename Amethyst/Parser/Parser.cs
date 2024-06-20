using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private IList<Token> Tokens { get; }
    private Namespace Context { get; }
    private int current;
    
    public Parser(IList<Token> tokens, Namespace context)
    {
        Tokens = tokens;
        Context = context;
        current = 0;
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
    
    private bool Check(params TokenType[] types)
    {
        if (IsAtEnd)
        {
            return false;
        }

        foreach (var type in types)
        {
            if (Peek().Type == type)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private bool Match(params TokenType[] types)
    {
        if (Check(types))
        {
            Advance();
            return true;
        }
        
        return false;
    }
    
    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        
        throw new SyntaxException(message, Peek().Line, Context.SourcePath);
    }

    public Stmt Parse()
    {
        var statements = new List<Stmt>();
        
        while (!IsAtEnd)
        {
            statements.Add(Declaration());
        }
        
        return new Stmt.Namespace
        {
            Name = new Token
            {
                Lexeme = Context.Name,
                Line = 0,
                Literal = Context.Name,
                Type = TokenType.IDENTIFIER
            },
            Body = new Stmt.Block
            {
                Statements = statements
            }
        };
    }
}