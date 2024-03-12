namespace Amethyst;

public static partial class Lexer
{
    private static readonly IReadOnlyDictionary<string, TokenType> TOKENS = new Dictionary<string, TokenType>
    {
        { "function", TokenType.KWD_FUNCTION },
        { "ticking", TokenType.KWD_TICKING },
        { "initializing", TokenType.KWD_INITIALIZING },
        { "namespace", TokenType.KWD_NAMESPACE },
        { "var", TokenType.KWD_VARIABLE },
        { "(", TokenType.PAREN_OPEN },
        { ")", TokenType.PAREN_CLOSE },
        { "{", TokenType.BRACE_OPEN },
        { "}", TokenType.BRACE_CLOSE },
        { "[", TokenType.BRACKET_OPEN },
        { "]", TokenType.BRACKET_CLOSE },
        { ",", TokenType.COMMA },
        { ";", TokenType.SEMICOLON },
        { "+", TokenType.OP_ADD },
        { "-", TokenType.OP_SUB },
        { "*", TokenType.OP_MUL },
        { "/", TokenType.OP_DIV },
        { "%", TokenType.OP_MOD },
        { "=", TokenType.OP_ASSIGN },
    };
    
    private const string DELIMITER = " (){}[];+-*=/%\\.:,#\n";
    
    public static IList<(TokenType, string)> Tokenize(string input)
    {
        var tokens = new List<(TokenType, string)>();
        int i = 0;
        string token = string.Empty;
        while (i < input.Length)
        {
            // these are tokens that may need to end early (through a delimiter)
            var c = input[i];
            if (DELIMITER.Contains(c))
            {
                if (TOKENS.TryGetValue(token, out var type))
                {
                    tokens.Add((type, token));
                    token = string.Empty;
                }
                else if (int.TryParse(token, out _))
                { 
                    tokens.Add((TokenType.LITERAL_NUMBER, token));
                    token = string.Empty;
                }
                else if (!string.IsNullOrWhiteSpace(token))
                {
                    tokens.Add((TokenType.IDENTIFIER, token));
                    token = string.Empty;
                }
            }
            
            token += c;
            
            // these are tokens that are matched immediately (strings, comments, etc.)
            if (string.IsNullOrWhiteSpace(token))
            {
                token = string.Empty;
            }
            else if (input[i] == '"')
            {
                token = string.Empty;
                i++;
                while (i < input.Length && input[i] != '"')
                {
                    token += input[i];
                    i++;
                }
                tokens.Add((TokenType.LITERAL_STRING, token));
                token = string.Empty;
            }
            else if (input[i] == '#')
            {
                while (i < input.Length && input[i] != '\n')
                {
                    i++;
                }
                token = string.Empty;
            }
            else if (TOKENS.TryGetValue(token, out var type))
            {
                tokens.Add((type, token));
                token = string.Empty;
            }
            
            i++;
        }
        
        return tokens;
    }
}