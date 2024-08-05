using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override BoolResult VisitBoolean_literal(AmethystParser.Boolean_literalContext context)
    {
        var number = context.GetText() == "true" ? 1 : 0;
        
        AddCode($"scoreboard players set {MemoryLocation} amethyst {number}");
        
        return new BoolResult
        {
            Compiler = this,
            Location = MemoryLocation++.ToString(),
            Context = context
        };
    }
}