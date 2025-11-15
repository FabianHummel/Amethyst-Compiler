using Antlr4.Runtime;

namespace Amethyst;

/// <summary>Exception representing semantic errors encountered during compilation. Semantic errors are
/// issues related to the meaning and logic of the code, such as type mismatches, undeclared variables,
/// and invalid operations, but not syntax errors.</summary>
/// <seealso cref="SyntaxException" />
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