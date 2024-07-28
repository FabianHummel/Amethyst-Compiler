using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitEquality(AmethystParser.EqualityContext context)
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
            
        for (int i = 1; i < context.comparison().Length; i++)
        {
            if (VisitComparison(comparisonContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected comparison expression.", comparisonContexts[i]);
            }
            
            var operatorToken = context.GetChild(2 * i - 1).GetText();

            MemoryLocation++;
            
            if (previous.DataType.IsScoreboardType && current.DataType.IsScoreboardType)
            {
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
            
                AddCode($"execute store result score {MemoryLocation} amethyst run execute if score {MemoryLocation} amethyst = {currentLocation} amethyst");
            }
            else if (previous.DataType.IsScoreboardType && current.DataType.IsStorageType)
            {
                AddCode($"execute store result storage amethyst: {MemoryLocation} {previous.DataType.StorageModifier} run scoreboard players get {previous.Location} amethyst");
                AddCode($"execute store success scoreboard {MemoryLocation} amethyst run data modify storage amethyst: {MemoryLocation} set from storage amethyst: {current.Location}");
            }
            else if (previous.DataType.IsStorageType && current.DataType.IsScoreboardType)
            {
                AddCode($"execute store result storage amethyst: {MemoryLocation} {current.DataType.StorageModifier} run scoreboard players get {current.Location} amethyst");
                AddCode($"execute store success scoreboard {MemoryLocation} amethyst run data modify storage amethyst: {MemoryLocation} set from storage amethyst: {previous.Location}");
            }
            else if (previous.DataType.IsStorageType && current.DataType.IsStorageType)
            {
                AddCode($"data modify storage amethyst: {MemoryLocation} set from storage amethyst: {previous.Location}");
                AddCode($"execute store success score {MemoryLocation} amethyst run data modify storage amethyst: {MemoryLocation} set from storage amethyst: {current.Location}");
            }

            // we need to invert the result, because store success ... modify set from ... will only yield true if the value is different
            if ((previous.DataType.IsStorageType || current.DataType.IsStorageType) && operatorToken == "==")
            {
                AddCode($"execute store success score {MemoryLocation} amethyst if score {MemoryLocation} amethyst matches 0");
            }
            
            previous = current;
        }
        
        return new BoolResult
        {
            Location = MemoryLocation--.ToString(),
            Compiler = this
        };
    }
}