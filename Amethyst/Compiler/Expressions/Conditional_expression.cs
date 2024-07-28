using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitConditional(AmethystParser.ConditionalContext context)
    {
        if (context.or() is not { } orContext)
        {
            throw new UnreachableException();
        }
        
        return VisitOr(orContext);
    }
}