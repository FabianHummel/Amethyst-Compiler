using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitDebugStatement(AmethystParser.DebugStatementContext context)
    {
        var result = VisitExpression(context.expression());

        string jsonComponent = null!;
        
        if (result is IConstantValue constantValue)
        {
            jsonComponent = constantValue.ToTextComponent();
        }

        if (result is IRuntimeValue runtimeValue)
        {
            if (runtimeValue.Datatype is AbstractScoreboardDatatype scoreboardDatatype)
            {
                AddCode($"execute store result storage amethyst:internal data.stringify.in {scoreboardDatatype.StorageModifier} run scoreboard players get {runtimeValue.Location}");
            }
            else
            {
                AddCode($"data modify storage amethyst:internal data.stringify.in set from storage {runtimeValue.Location}");
            }
            AddCode("function amethyst:internal/data/stringify/run");
            
            jsonComponent = """{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}""";
        }
        
        AddCode($$"""tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{{jsonComponent}}]""");
        
        return null;
    }
}