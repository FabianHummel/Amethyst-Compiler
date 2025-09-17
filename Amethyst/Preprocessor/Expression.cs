using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override PreprocessorResult VisitPreprocessorExpression(AmethystParser.PreprocessorExpressionContext context)
    {
        return (PreprocessorResult)Visit(context)!;
    }
}