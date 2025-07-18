using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFile(AmethystParser.FileContext context)
    {
        if (context.namespace_declaration() is { } namespaceDeclaration)
        {
            VisitNamespace_declaration(namespaceDeclaration);
        }
        foreach (var declarationContext in context.declaration())
        {
            VisitDeclaration(declarationContext);
        }
        
        return null;
    }
}