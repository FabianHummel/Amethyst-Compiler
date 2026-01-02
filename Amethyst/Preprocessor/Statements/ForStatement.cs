using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Loops over a block of code while a specific condition is true during preprocessing.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitForStatement" />
    public override object? VisitPreprocessorForStatement(AmethystParser.PreprocessorForStatementContext context)
    {
        if (context.preprocessorForeachStatementInitializer() is { } foreachStatementInitializerContext)
        {
            ProcessForEachLoop(context, foreachStatementInitializerContext);
        }
        else
        {
            ProcessForLoop(context);
        }

        return null;
    }
    
    private void ProcessForEachLoop(AmethystParser.PreprocessorForStatementContext context, AmethystParser.PreprocessorForeachStatementInitializerContext foreachStatementInitializerContext)
    {
        var iterable = VisitPreprocessorExpression(foreachStatementInitializerContext.preprocessorExpression());
        if (iterable is not PreprocessorArray array)
        {
            throw new SyntaxException("The expression in a for-in loop must be iterable.", context);
        }
        
        var variableName = foreachStatementInitializerContext.IDENTIFIER().GetText();
        
        foreach (var value in array.Value)
        {
            var preprocessorValue = (AbstractPreprocessorValue)value;
            
            Scope.Symbols.Add(variableName, new PreprocessorVariable
            {
                Value = preprocessorValue
            });
            
            VisitBlockInline(context.block());
            
            Scope.Symbols.Remove(variableName);
        }
    }
    
    private void ProcessForLoop(AmethystParser.PreprocessorForStatementContext context)
    {
        Visit(context.preprocessorForStatementInitializer());

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
                VisitBlockInline(context.block());
                if (context.preprocessorAssignment() is { } preprocessorAssignmentContext)
                {
                    Visit(preprocessorAssignmentContext);
                }
                value = VisitPreprocessorExpression(context.preprocessorExpression());
            }
        });
    }
}