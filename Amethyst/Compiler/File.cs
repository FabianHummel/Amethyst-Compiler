using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFile(AmethystParser.FileContext context)
    {
        foreach (var declarationContext in context.declaration())
        {
            VisitDeclaration(declarationContext);
        }
        
        return null;
    }
}