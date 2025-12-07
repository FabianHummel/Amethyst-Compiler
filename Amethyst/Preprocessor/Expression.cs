using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Utility function to visit any preprocessor expression and return its result as a
    /// <see cref="AbstractPreprocessorValue" />.</summary>
    public AbstractPreprocessorValue VisitPreprocessorExpression(AmethystParser.PreprocessorExpressionContext context)
    {
        return (AbstractPreprocessorValue)Visit(context)!;
    }
}