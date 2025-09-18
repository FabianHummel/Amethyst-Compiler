using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorIfStatement(AmethystParser.PreprocessorIfStatementContext context)
    {
        var blockContexts = context.block();
        var result = VisitPreprocessorExpression(context.preprocessorExpression());

        if (result.AsBoolean)
        {
            return VisitBlock(blockContexts[0]);
        }
        else if (blockContexts.Length == 2)
        {
            return VisitBlock(blockContexts[1]);
        }
        else if (context.preprocessorIfStatement() is PreprocessorIfStatementContext preprocessorIfStatementContext)
        {
            return VisitPreprocessorIfStatement(preprocessorIfStatementContext);
        }

        return null;
    }
}