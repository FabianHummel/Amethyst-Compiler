using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitBlock(AmethystParser.BlockContext context)
    {
        var mcFunctionPath = VisitBlockNamed(context, "_block");
        this.AddCode($"function {mcFunctionPath}");
        return null;
    }
    
    public void VisitBlockInline(AmethystParser.BlockContext context)
    {
        foreach (var preprocessorStatementContext in context.preprocessorStatement())
        {
            Visit(preprocessorStatementContext);
        }
    }

    internal string VisitBlockNamed(AmethystParser.BlockContext context, string name, bool preserveName = false)
    {
        using var scope = this.EvaluateScoped(name, preserveName);
        VisitBlockInline(context);
        return Scope.McFunctionPath;
    }
}