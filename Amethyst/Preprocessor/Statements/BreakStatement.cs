using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
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