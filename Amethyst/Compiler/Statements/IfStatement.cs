using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitIfStatement(AmethystParser.IfStatementContext context)
    {
        var blockContexts = context.block();
        var result = VisitExpression(context.expression());

        if (result is ConstantBoolean booleanConstant)
        {
            if (booleanConstant.Value)
            {
                VisitBlockInline(blockContexts[0]);
            }
            
            return null;
        }
        
        if (result is RuntimeBoolean booleanResult)
        {
            var mcFunctionPath = VisitBlockNamed(blockContexts[0], "_func");
            this.AddCode($"execute if score {booleanResult.Location} matches 1 run function {mcFunctionPath}");
            return null;
        }
        
        // TODO: else branch
        
        throw new SyntaxException("Expected boolean expression in if statement", context.expression());
    }
}