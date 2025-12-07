using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Breaks out of the enclosing preprocessor loop.</p><p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">There is no enclosing loop to break out of.</exception>
    /// <seealso cref="VisitBreakStatement" />
    public override object? VisitPreprocessorBreakStatement(AmethystParser.PreprocessorBreakStatementContext context)
    {
        if (LoopingScope is not { } scope)
        {
            throw new SyntaxException("Can only break within enclosing loop.", context);
        }

        scope.Break();

        return null;
    }
}