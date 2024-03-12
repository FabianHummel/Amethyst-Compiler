namespace Amethyst;

public class SyntaxException : Exception
{
    public Token Expected { get; private set; }
    
    public SyntaxException(string message, Token token) : base(message)
    {
        Expected = token;
    }
}