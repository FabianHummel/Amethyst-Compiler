using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Breaks out of the enclosing preprocessor loop. <br /><inheritdoc /></summary>
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