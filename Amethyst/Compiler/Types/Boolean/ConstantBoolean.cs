using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class ConstantBoolean : AbstractBoolean, IConstantValue<bool>, IScoreboardValue
{
    public required bool Value { get; init; }
    
    public bool AsBoolean => Value;
    
    public int AsInteger => AsBoolean ? 1 : 0;
    
    protected override AbstractDecimal AsDecimal => new ConstantDecimal
    {
        Compiler = Compiler,
        Context = Context,
        Value = AsInteger,
        DecimalPlaces = 0
    };

    public int ScoreboardValue => AsInteger;

    public IRuntimeValue ToRuntimeValue()
    {
        var location = Location.Scoreboard(++Compiler.StackPointer);
        
        AddCode($"scoreboard players set {location} {AsInteger}");
        
        return new RuntimeBoolean
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }

    public string ToNbtString()
    {
        return Value.ToNbtString();
    }

    public string ToTextComponent()
    {
        var value = Value ? "1" : "0";
        return $$"""[{"text":"{{value}}","color":"gold"},{"text":"b","color":"red"}]""";
    }

    public bool Equals(IConstantValue? other)
    {
        if (other is not ConstantBoolean booleanConstant)
        {
            return false;
        }

        return Value.Equals(booleanConstant.Value);
    }

    public override string ToTargetSelectorString()
    {
        return Value ? "true" : "false";
    }
}