using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Loops over a block of code while a specific condition is true during preprocessing. <br />
    /// <inheritdoc /></summary>
    /// <seealso cref="VisitForStatement" />
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
                if (context.preprocessorExpressionStatement() is { } preprocessorExpressionStatementContext)
                {
                    VisitPreprocessorExpressionStatement(preprocessorExpressionStatementContext);
                }
                value = VisitPreprocessorExpression(context.preprocessorExpression());
            }
        });

        return null;
    }
}