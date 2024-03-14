namespace Amethyst;

public class SyntaxException : Exception
{
    public Token? Expected { get; private set; }
    
    public SyntaxException(string message, Token? token = null) : base(message)
    {
        Expected = token;
    }
}