namespace Amethyst.Model;

public class SyntaxException : Exception
{
    public int Line { get; }
    public int PosInLine { get; }
    public string File { get; }
    
    public SyntaxException(string message, int line, int posInLine, string file) : base(message)
    {
        Line = line;
        PosInLine = posInLine;
        File = file;
    }
}