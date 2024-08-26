using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitComparison_expression(AmethystParser.Comparison_expressionContext context)
    {
        if (context.term_expression() is not { } termExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (termExpressionContexts.Length == 1)
        {
            return VisitTerm_expression(termExpressionContexts[0]);
        }
        
        if (VisitTerm_expression(termExpressionContexts[0]) is not { DataType.IsScoreboardType: true } previous)
        {
            throw new SyntaxException("Expected term expression.", termExpressionContexts[0]);
        }
        
        for (int i = 1; i < context.term_expression().Length; i++)
        {
            var current = VisitTerm_expression(termExpressionContexts[i]).MakeNumber();
            
            var operatorToken = context.GetChild(2 * i - 1).GetText();
            
            // upscale to a common denominator

            MemoryLocation++;
            AddCode($"scoreboard players operation {MemoryLocation} amethyst = {previous.Location} amethyst"); // Todo: See #2

            if (current.DataType.Scale != 1)
            {
                AddCode($"scoreboard players operation {MemoryLocation} amethyst *= .{current.DataType.Scale} amethyst_const");
            }
            
            var currentLocation = current.Location;

            if (previous.DataType.Scale != 1)
            {
                MemoryLocation++;
                AddCode($"scoreboard players operation {MemoryLocation} amethyst = {current.Location} amethyst"); // Todo: See #2
                AddCode($"scoreboard players operation {MemoryLocation} amethyst *= .{previous.DataType.Scale} amethyst_const");
                currentLocation = MemoryLocation.ToString();
                MemoryLocation--;
            }
            
            AddCode($"execute store result score {MemoryLocation} amethyst run execute if score {MemoryLocation} amethyst {operatorToken} {currentLocation} amethyst");
            
            previous = current;
        }
        
        return new BooleanResult
        {
            Location = MemoryLocation--.ToString(),
            Compiler = this,
            Context = context
        };
    }
}