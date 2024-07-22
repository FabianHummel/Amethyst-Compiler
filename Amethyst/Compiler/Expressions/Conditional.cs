using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public Result VisitConditionalTargeted(AmethystParser.ConditionalContext context, string target)
    {
        if (context.or() is { } orContext)
        {
            return VisitOrTargeted(orContext, target: target);
        }
        
        throw new UnreachableException();
    }
}