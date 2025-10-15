using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitBlock(AmethystParser.BlockContext context)
    {
        var mcFunctionPath = VisitBlockNamed(context, "_block");
        AddCode($"function {mcFunctionPath}");
        return null;
    }
    
    private string VisitBlockNamed(AmethystParser.BlockContext context, string name, Action? init = null)
    {
        return VisitBlockNamed(context, name, _ => init?.Invoke());
    }
    
    private string VisitBlockNamed(AmethystParser.BlockContext context, string name, Action<string>? init)
    {
        return EvaluateScoped(name, (scope, _) =>
        {
            init?.Invoke(scope.McFunctionPath);
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