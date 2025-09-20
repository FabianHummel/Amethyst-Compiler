using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorForStatement(AmethystParser.PreprocessorForStatementContext context)
    {
        if (context.preprocessorVariableDeclaration() is { } preprocessorVariableDeclarationContext)
        {
            VisitPreprocessorVariableDeclaration(preprocessorVariableDeclarationContext);
        }
        else if (context.preprocessorExpressionStatement() is { } preprocessorExpressionStatementContext)
        {
            VisitPreprocessorExpressionStatement(preprocessorExpressionStatementContext);
        }

        var result = VisitPreprocessorExpression(context.preprocessorExpression());

        var maxIterations = 10_000_000;

        using var scope = new LoopingScope(this, () =>
        {
            while(result.AsBoolean && maxIterations-- > 0)
            {
                VisitBlock(context.block());
                result = VisitPreprocessorExpression(context.preprocessorExpression());
            }
        });

        return null;
    }
}