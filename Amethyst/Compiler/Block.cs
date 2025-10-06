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
    
    private Scope VisitBlockNamed(AmethystParser.BlockContext context, string name, Action? init = null)
    {
        return EvaluateScoped(name, _ =>
        {
            init?.Invoke();
            VisitBlockInline(context);
        });
    }
    
    private void VisitBlockInline(AmethystParser.BlockContext context)
    {
        foreach (var preprocessorStatementContext in context.preprocessorStatement())
        {
            Visit(preprocessorStatementContext);
        }
    }
}