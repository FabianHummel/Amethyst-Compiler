using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorDebugStatement(AmethystParser.PreprocessorDebugStatementContext context)
    {
        var result = VisitPreprocessorExpression(context.preprocessorExpression());
        Console.WriteLine(result.ToString());
        return null;
    }
}