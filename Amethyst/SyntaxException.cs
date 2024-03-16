namespace Amethyst;

public class SyntaxException : Exception
{
    public int Line { get; }
    
    public SyntaxException(string message, int line) : base(message)
    {
        Line = line;
    }
}