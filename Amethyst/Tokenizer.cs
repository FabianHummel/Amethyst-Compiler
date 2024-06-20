using Amethyst.Model;

namespace Amethyst;

public class Tokenizer
{
    private string Input { get; }
    private Namespace Context { get; }
    private IList<Token> Tokens { get; } = new List<Token>();
 
    public Tokenizer(string input, Namespace context)
    {
        Input = input;
        Context = context;
    }
    
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private TokenType intOrDecimal;

    private static readonly Dictionary<string, TokenType> KEYWORDS = new()
    {
        { "if", TokenType.IF }, 
        { "else", TokenType.ELSE },
        { "for", TokenType.FOR },
        { "foreach", TokenType.FOREACH },
        { "while", TokenType.WHILE },
        { "and", TokenType.AND },
        { "or", TokenType.OR },
        { "xor", TokenType.XOR },
        { "break", TokenType.BREAK },
        { "continue", TokenType.CONTINUE },
        { "return", TokenType.RETURN },
        { "true", TokenType.TRUE },
        { "false", TokenType.FALSE },
        { "function", TokenType.FUNCTION },
        { "namespace", TokenType.NAMESPACE },
        { "var", TokenType.VAR },
        { "record", TokenType.RECORD },
        { "debug", TokenType.DEBUG },
        { "comment", TokenType.COMMENT },
        { "string", TokenType.DT_STRING },
        { "int", TokenType.DT_INT },
        { "dec", TokenType.DT_DEC },
        { "bool", TokenType.DT_BOOL },
        { "array", TokenType.DT_ARRAY },
        { "object", TokenType.DT_OBJECT }
    };

    private bool IsAtEnd => current >= Input.Length;
    
    private char Advance()
    {
        return Input[current++];
    }
    
    private bool Match(char expected)
    {
        if (IsAtEnd) return false;
        if (Input[current] != expected) return false;

        current++;
        return true;
    }
    
    private char Peek()
    {
        if (IsAtEnd) return '\0';
        return Input[current];
    }
    
    private char PeekNext()
    {
        if (current + 1 >= Input.Length) return '\0';
        return Input[current + 1];
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
            throw new SyntaxException("Unterminated string", line, Context.SourcePath);
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = Input[(start + 1)..(current - 1)];
        AddToken(TokenType.STRING, value);
    }
    
    private void Number()
    {
        intOrDecimal = TokenType.INTEGER;
        while (IsDigit(Peek())) Advance();

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext())) {
            intOrDecimal = TokenType.DECIMAL;
            // Consume the "."
            Advance();
            while (IsDigit(Peek())) Advance();
        }

        AddToken(intOrDecimal, double.Parse(Input[start..current]));
    }
    
    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = Input[start..current]; 
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
        string text = Input[start..current];
        Tokens.Add(new Token
        {
            Type = type,
            Lexeme = text,
            Literal = literal!,
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
                    throw new SyntaxException($"Unexpected character '{c}'", line, Context.SourcePath);
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
            Literal = null!,
            Line = line
        });
        
        return Tokens;
    }
}