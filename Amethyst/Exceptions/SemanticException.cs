using Antlr4.Runtime;

namespace Amethyst;

public class SemanticException : Exception
{
    public int Line { get; }
    public int Column { get; }
    public string File { get; }
    
    public SemanticException(string message, int line, int column, string file) : base(message)
    {
        Line = line;
        Column = column;
        File = file;
    }
    
    public SemanticException(string message, ParserRuleContext context) : base(message)
    {
        Line = context.Start.Line;
        Column = context.Start.Column;
        File = context.Start.InputStream.SourceName;
    }
}