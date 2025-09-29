using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractPreprocessorValue VisitPreprocessorExpression(AmethystParser.PreprocessorExpressionContext context)
    {
        return (AbstractPreprocessorValue)Visit(context)!;
    }
}