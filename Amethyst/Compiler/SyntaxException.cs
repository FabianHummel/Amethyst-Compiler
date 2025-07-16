using Antlr4.Runtime;

namespace Amethyst;

public class SyntaxException : Exception
{
    public int Line { get; }
    public int Column { get; }
    public string File { get; }
    
    public SyntaxException(string message, int line, int column, string file) : base(message)
    {
        Line = line;
        Column = column;
        File = file;
    }

    public SyntaxException(string message, ParserRuleContext context) : base(message)
    {
        Line = context.Start.Line;
        Column = context.Start.Column;
        File = context.Start.InputStream.SourceName;
    }
}