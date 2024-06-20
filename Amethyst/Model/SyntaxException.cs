namespace Amethyst.Model;

public class SyntaxException : Exception
{
    public int Line { get; }
    public string File { get; }
    
    public SyntaxException(string message, int line, string file) : base(message)
    {
        Line = line;
        File = file;
    }
}