using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override Symbol VisitDeclaration(AmethystParser.DeclarationContext context)
    {
        return (Symbol)base.VisitDeclaration(context)!;
    }
}