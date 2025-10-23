using Antlr4.Runtime;

namespace Amethyst;

public class SemanticException : AmethystException
{
    public SemanticException(string message, int line, int column, string file) : base(
        $"Semantic error: {file} ({line}:{column}): {message}")
    {
    }
    
    public SemanticException(string message, ParserRuleContext context) : this(
        message, context.Start.Line, context.Start.Column, context.Start.InputStream.SourceName)
    {
    }
}