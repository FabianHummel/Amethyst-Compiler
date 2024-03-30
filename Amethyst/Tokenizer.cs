namespace Amethyst;

public class Tokenizer
{
    public Tokenizer(string input)
    {
        Source = input;
    }

    private string Source { get; }
    private IList<Token> Tokens { get; } = new List<Token>();
    
    private int start = 0;
    private int current = 0;
    private int line = 1;

    private static readonly Dictionary<string, TokenType> KEYWORDS = new()
    {
        { "and",            TokenType.AND },
        { "else",           TokenType.ELSE },
        { "false",          TokenType.FALSE },
        { "function",       TokenType.FUNCTION },
        { "ticking",        TokenType.TICKING },
        { "initializing",   TokenType.INITIALIZING },
        { "namespace",      TokenType.NAMESPACE },
        { "for",            TokenType.FOR },
        { "if",             TokenType.IF },
        { "null",           TokenType.NULL },
        { "or",             TokenType.OR },
        { "break",          TokenType.BREAK },
        { "return",         TokenType.RETURN },
        { "true",           TokenType.TRUE },
        { "var",            TokenType.VAR },
        { "while",          TokenType.WHILE },
        { "print",          TokenType.PRINT },
        { "comment",        TokenType.COMMENT },
        { "continue",       TokenType.CONTINUE },
        
        { "FUNCTION",       TokenType.FUNCTION },
        { "NAMESPACE",      TokenType.NAMESPACE },
        { "FOR",            TokenType.FOR },
        { "IF",             TokenType.IF },
        { "ELSE",           TokenType.ELSE },
        { "BREAK",          TokenType.BREAK },
        { "RETURN",         TokenType.RETURN },
        { "VAR",            TokenType.VAR },
        { "WHILE",          TokenType.WHILE },
        { "PRINT",          TokenType.PRINT },
        { "CONTINUE",       TokenType.CONTINUE }
    };

    private bool IsAtEnd => current >= Source.Length;
    
    private char Advance()
    {
        return Source[current++];
    }
    
    private bool Match(char expected)
    {
        if (IsAtEnd) return false;
        if (Source[current] != expected) return false;

        current++;
        return true;
    }
    
    private char Peek()
    {
        if (IsAtEnd) return '\0';
        return Source[current];
    }
    
    private char PeekNext()
    {
        if (current + 1 >= Source.Length) return '\0';
        return Source[current + 1];
    }
    
    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }
    
    private bool IsAlpha(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }
    
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }
    
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd) {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd) {
            throw new SyntaxException("Unterminated string.", line);
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = Source[(start + 1)..(current - 1)];
        AddToken(TokenType.STRING, value);
    }
    
    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext())) {
            // Consume the "."
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.NUMBER, double.Parse(Source[start..current]));
    }
    
    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = Source[start..current]; 
        if (KEYWORDS.TryGetValue(text, out var type))
        {
            AddToken(type);
        }
        else
        {
            AddToken(TokenType.IDENTIFIER);
        }
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = Source[start..current];
        Tokens.Add(new Token
        {
            Type = type,
            Lexeme = text,
            Literal = literal,
            Line = line
        });
    }
    
    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case '[': AddToken(TokenType.LEFT_BRACKET); break;
            case ']': AddToken(TokenType.RIGHT_BRACKET); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case ':': AddToken(TokenType.COLON); break;
            
            case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
            case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
            case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
            case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
            case '+': AddToken(Match('=') ? TokenType.PLUS_EQUAL : TokenType.PLUS); break;
            case '-': AddToken(Match('=') ? TokenType.MINUS_EQUAL : TokenType.MINUS); break;
            case '/': AddToken(Match('=') ? TokenType.SLASH_EQUAL : TokenType.SLASH); break;
            case '*': AddToken(Match('=') ? TokenType.STAR_EQUAL : TokenType.STAR); break;
            case '%': AddToken(Match('=') ? TokenType.MODULO_EQUAL : TokenType.MODULO); break;
            
            case '#':
                while (Peek() != '\n' && !IsAtEnd) Advance();
                break;
            
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;
            
            case '\n':
                line++;
                break;
            
            case '"':
                String();
                break;
            
            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    throw new SyntaxException($"Unexpected character '{c}'", line);
                }
                break;
        }
    }

    public IList<Token> Tokenize()
    {
        while (!IsAtEnd)
        {
            // We are at the beginning of the next lexeme.
            start = current;
            ScanToken();
        }

        Tokens.Add(new Token
        {
            Type = TokenType.EOF,
            Lexeme = "",
            Literal = null,
            Line = line
        });
        
        return Tokens;
    }
}