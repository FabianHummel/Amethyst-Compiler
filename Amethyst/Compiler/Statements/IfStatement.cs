using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Parses an if-else branch. Internally, the condition is evaluated and stored into as a
    ///     boolean flag in the scoreboard. Depending on the resulting value, the respective function is
    ///     called.</p>
    ///     <p><inheritdoc /></p></summary>
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
            this.AddCode($"execute if score {booleanResult.Location} matches 1 run function {mcFunctionPath}"); // BUG: I don't think this works with "break", "continue" and "return"
            return null;
        }
        
        // TODO: else branch
        
        throw new SyntaxException("Expected boolean expression in if statement", context.expression());
    }
}