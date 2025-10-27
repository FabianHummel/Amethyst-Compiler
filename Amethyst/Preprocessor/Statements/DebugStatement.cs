using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Prints a value to the console during preprocessing. <br /><inheritdoc /></summary>
    /// <seealso cref="VisitDebugStatement" />
    public override object? VisitPreprocessorDebugStatement(AmethystParser.PreprocessorDebugStatementContext context)
    {
        var result = VisitPreprocessorExpression(context.preprocessorExpression());
        Console.WriteLine(result.ToString());
        return null;
    }
}