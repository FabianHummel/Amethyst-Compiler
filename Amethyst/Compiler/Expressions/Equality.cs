using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    public override Result VisitEquality(AmethystParser.EqualityContext context)
    {
        if (context.comparison() is not { } comparisonContexts)
        {
            throw new UnreachableException();
        }
        
        if (comparisonContexts.Length == 1)
        {
            return VisitComparison(comparisonContexts[0]);
        }
            
        if (VisitComparison(comparisonContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected comparison expression.", comparisonContexts[0]);
        }
            
        foreach (var comparisonContext in comparisonContexts)
        {
            if (VisitComparison(comparisonContext) is not { } current)
            {
                throw new SyntaxException("Expected comparison expression.", comparisonContext);
            }
                
            if (previous.Type.IsScoreboardType && current.Type.IsScoreboardType)
            {
            }
            else if (previous.Type.IsScoreboardType && current.Type.IsStorageType)
            {
            }
            else if (previous.Type.IsStorageType && current.Type.IsScoreboardType)
            {
            }
            else if (previous.Type.IsStorageType && current.Type.IsStorageType)
            {
            }
            
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