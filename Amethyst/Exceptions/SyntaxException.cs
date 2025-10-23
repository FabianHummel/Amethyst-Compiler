using Antlr4.Runtime;

namespace Amethyst;

public class SyntaxException : AmethystException
{
    public SyntaxException(string message, int line, int column, string file) : base(
        $"Syntax error: {file} ({line}:{column}): {message}")
    {
    }

    public SyntaxException(string message, ParserRuleContext context) : this(
        message, context.Start.Line, context.Start.Column, context.Start.InputStream.SourceName)
    {
    }
}