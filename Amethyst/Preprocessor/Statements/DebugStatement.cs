using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Prints a value to the console during preprocessing.</p><p><inheritdoc /></p></summary>
    /// <seealso cref="VisitDebugStatement" />
    public override object? VisitPreprocessorDebugStatement(AmethystParser.PreprocessorDebugStatementContext context)
    {
        var result = VisitPreprocessorExpression(context.preprocessorExpression());
        Console.WriteLine(result.ToString());
        return null;
    }
}