using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorForStatement(AmethystParser.PreprocessorForStatementContext context)
    {
        if (context.preprocessorVariableDeclaration() is PreprocessorVariableDeclaration preprocessorVariableDeclarationContext)
        {
            VisitPreprocessorVariableDeclaration(preprocessorVariableDeclarationContext);
        }
        else if (context.preprocessorExpressionStatement() is PreprocessorExpressionStatement preprocessorExpressionStatementContext)
        {
            VisitPreprocessorExpressionStatement(preprocessorExpressionStatementContext);
        }

        var result = VisitPreprocessorExpression(context.preprocessorExpression());

        var maxIterations = 10_000_000;

        using var scope = new LoopingScope(this);

        while(result.AsBoolean && maxIterations-- > 0)
        {
            VisitBlock(context.block());
            result = VisitPreprocessorExpression(context.preprocessorExpression());

            if (scope.IsCancelled)
            {
                break;
            }
        }

        return null;
    }
}