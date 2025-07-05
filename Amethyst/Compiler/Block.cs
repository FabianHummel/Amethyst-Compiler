using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitBlock(AmethystParser.BlockContext context)
    {
        var scope = VisitBlockNamed(context, "_block");
        AddCode($"function {scope.McFunctionPath}");
        return null;
    }

    private Scope VisitBlockNamed(AmethystParser.BlockContext context, string name)
    {
        return EvaluateScoped(name, _ =>
        {
            foreach (var statementContext in context.statement())
            {
                VisitStatement(statementContext);
            }
        });
    }
}