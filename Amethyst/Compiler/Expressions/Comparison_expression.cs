using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitComparison(AmethystParser.ComparisonContext context)
    {
        if (context.term() is not { } termContexts)
        {
            throw new UnreachableException();
        }
        
        if (termContexts.Length == 1)
        {
            return VisitTerm(termContexts[0]);
        }
        
        if (VisitTerm(termContexts[0]) is not { DataType.IsScoreboardType: true } previous)
        {
            throw new SyntaxException("Expected term expression.", termContexts[0]);
        }
        
        for (int i = 1; i < context.term().Length; i++)
        {
            if (VisitTerm(termContexts[i]).ToNumber is not { DataType.IsScoreboardType: true } current)
            {
                throw new SyntaxException("Expected term expression.", termContexts[i]);
            }
            
            var operatorToken = context.GetChild(2 * i - 1).GetText();
            
            // upscale to a common denominator

            MemoryLocation++;
            AddCode($"scoreboard players operation {MemoryLocation} amethyst = {previous.Location} amethyst");

            if (current.DataType.Scale != 1)
            {
                AddCode($"scoreboard players operation {MemoryLocation} amethyst *= .{current.DataType.Scale} amethyst_const");
            }
            
            var currentLocation = current.Location;

            if (previous.DataType.Scale != 1)
            {
                MemoryLocation++;
                AddCode($"scoreboard players operation {MemoryLocation} amethyst = {current.Location} amethyst");
                AddCode($"scoreboard players operation {MemoryLocation} amethyst *= .{previous.DataType.Scale} amethyst_const");
                currentLocation = MemoryLocation.ToString();
                MemoryLocation--;
            }
            
            AddCode($"execute store result score {MemoryLocation} amethyst run execute if score {MemoryLocation} amethyst {operatorToken} {currentLocation} amethyst");
            
            previous = current;
        }
        
        return new BoolResult
        {
            Location = MemoryLocation--.ToString(),
            Compiler = this
        };
    }
}