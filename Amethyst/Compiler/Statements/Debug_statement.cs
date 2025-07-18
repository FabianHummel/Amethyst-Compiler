using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitDebug_statement(AmethystParser.Debug_statementContext context)
    {
        var result = VisitExpression(context.expression());

        string jsonComponent = null!;
        
        if (result is ConstantValue constantValue)
        {
            jsonComponent = constantValue.ToTextComponent();
        }

        if (result is RuntimeValue runtimeValue)
        {
            if (runtimeValue.DataType.Location == DataLocation.Scoreboard)
            {
                AddCode($"execute store result storage amethyst:internal data.stringify.in {runtimeValue.DataType.StorageModifier} run scoreboard players get {runtimeValue.Location} amethyst");
            }
            else
            {
                AddCode($"data modify storage amethyst:internal data.stringify.in set from storage amethyst: {runtimeValue.Location}");
            }
            
            AddCode("function amethyst:api/data/stringify");
            jsonComponent = """{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}""";
        }
        
        AddCode($$"""tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{{jsonComponent}}]""");
        
        return null;
    }
}