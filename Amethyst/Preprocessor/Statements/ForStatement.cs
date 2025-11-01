using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorForStatement(AmethystParser.PreprocessorForStatementContext context)
    {
        if (context.preprocessorForStatementInitializer() is { } forStatementInitializerContext)
        {
            Visit(forStatementInitializerContext);
        }

        AbstractPreprocessorValue value;
        if (context.preprocessorExpression() is { } expressionContext)
        {
            value = VisitPreprocessorExpression(expressionContext);
        }
        else
        {
            value = new PreprocessorBoolean
            {
                Compiler = this,
                Context = context,
                Value = true
            };
        }
        
        var maxIterations = 10_000_000;

        using var scope = new LoopingScope(this, () =>
        {
            while(value.AsBoolean && maxIterations-- > 0)
            {
                VisitBlock(context.block());
                if (context.preprocessorAssignment() is { } preprocessorAssignmentContext)
                {
                    Visit(preprocessorAssignmentContext);
                }
                value = VisitPreprocessorExpression(context.preprocessorExpression());
            }
        });

        return null;
    }
}