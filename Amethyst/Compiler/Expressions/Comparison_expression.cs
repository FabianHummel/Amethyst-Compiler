using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    public override Result VisitComparison(AmethystParser.ComparisonContext context)
    {
        if (context.term() is not { } termContexts)
        {
            throw new UnreachableException();
        }
        
        if (termContexts.Length == 1)
        {
            return VisitTerm(termContexts[0]);
        }
        
        if (VisitTerm(termContexts[0]) is not { Type.IsScoreboardType: true } previous)
        {
            throw new SyntaxException("Expected term expression.", termContexts[0]);
        }
        
        foreach (var termContext in termContexts)
        {
            if (VisitTerm(termContext) is not { Type.IsScoreboardType: true } current)
            {
                throw new SyntaxException("Expected term expression.", termContext);
            }
            
            // this somewhat relates to common denominators
            
            var scalePreviousBy = current.Type.Scale!;
            var scaleCurrentBy = previous.Type.Scale!;
            
            // after scaling, check for equality
            
            previous = current;
            
            MemoryLocation--;
        }
        
        return new Result
        {
            Location = MemoryLocation.ToString(),
            Type = new Type
            {
                BasicType = BasicType.Bool
            }
        };
    }
}