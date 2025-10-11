using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public AbstractPreprocessorValue VisitPreprocessorExpression(AmethystParser.PreprocessorExpressionContext context)
    {
        return (AbstractPreprocessorValue)Visit(context)!;
    }
}