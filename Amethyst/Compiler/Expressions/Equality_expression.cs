using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitEquality_expression(AmethystParser.Equality_expressionContext context)
    {
        if (context.comparison_expression() is not { } comparisonExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (comparisonExpressionContexts.Length == 1)
        {
            return VisitComparison_expression(comparisonExpressionContexts[0]);
        }
            
        if (VisitComparison_expression(comparisonExpressionContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected comparison expression.", comparisonExpressionContexts[0]);
        }
            
        for (int i = 1; i < context.comparison_expression().Length; i++)
        {
            if (VisitComparison_expression(comparisonExpressionContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected comparison expression.", comparisonExpressionContexts[i]);
            }
            
            var operatorToken = context.GetChild(2 * i - 1).GetText();
            
            if (previous.DataType != current.DataType)
            {
                AddCode($"scoreboard players set {MemoryLocation} amethyst {Convert.ToInt32(operatorToken == "!=")}");

                previous = new BoolResult
                {
                    Location = MemoryLocation.ToString(),
                    Compiler = this,
                    Context = context,
                    IsTemporary = true
                };
                
                continue;
            }
            
            if (previous.DataType.IsScoreboardType && current.DataType.IsScoreboardType)
            {
                var previousScaled = previous.ToNumber;
                var currentScaled = current.ToNumber;
                
                var conditionalClause = (operatorToken == "==").ToConditionalClause();

                var previousLocation = previousScaled.Location;
                if (!previousScaled.IsTemporary)
                {
                    previousLocation = MemoryLocation++.ToString();
                }
                
                AddCode($"execute store result score {previousLocation} amethyst run execute {conditionalClause} score {previousScaled.Location} amethyst = {currentScaled.Location} amethyst");
                
                previous = new BoolResult
                {
                    Location = previousLocation,
                    Compiler = this,
                    Context = context,
                    IsTemporary = true
                };
                
                continue;
            }
            
            if (previous.DataType.IsStorageType && current.DataType.IsStorageType)
            {
                var previousLocation = previous.Location;
                if (!previous.IsTemporary)
                {
                    previousLocation = MemoryLocation++.ToString();
                    AddCode($"data modify storage amethyst: {previousLocation} set from storage amethyst: {previous.Location}");
                }
                
                AddCode($"execute store success score {previousLocation} amethyst run data modify storage amethyst: {previousLocation} set from storage amethyst: {current.Location}");
                
                // we need to invert the result, because store success ... modify set from ... will yield true if the value is different
                if (operatorToken == "==")
                {
                    AddCode($"execute store success score {MemoryLocation} amethyst if score {MemoryLocation} amethyst matches 0");
                }
            }
            
            previous = current;
        }
        
        return new BoolResult
        {
            Location = previous.Location,
            Compiler = this,
            Context = context
        };
    }
}