using Antlr4.Runtime;

namespace Amethyst;

/// <summary>Exception representing syntax errors encountered during compilation. Syntax errors are
/// issues related to the structure and format of the code, such as missing semicolons, unmatched
/// parentheses, and incorrect keywords.</summary>
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