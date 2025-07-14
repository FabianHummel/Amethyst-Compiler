using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitIf_statement(AmethystParser.If_statementContext context)
    {
        var result = VisitExpression(context.expression());

        if (result is BooleanConstant booleanConstant)
        {
            if (booleanConstant.Value)
            {
                VisitBlockInline(context.block()[0]);
            }
            
            return null;
        }
        
        if (result is BooleanResult booleanResult)
        {
            var scope = VisitBlockNamed(context.block()[0], "_func");
            AddCode($"execute if score {booleanResult.Location} amethyst matches 1 run function {scope.McFunctionPath}");
            return null;
        }
        
        throw new SyntaxException("Expected boolean expression in if statement", context.expression());
    }
}