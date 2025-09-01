using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFile(AmethystParser.FileContext context)
    {
        foreach (var fromContext in context.from())
        {
            VisitFrom(fromContext);
        }
        
        foreach (var declarationContext in context.declaration())
        {
            VisitDeclaration(declarationContext);
        }
        
        return null;
    }
}