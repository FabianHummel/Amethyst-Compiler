using Amethyst.Model;

namespace Amethyst;

public class IntegerResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    protected override double ConstantValueAsDecimal => (int)ConstantValue!;

    public override BooleanResult MakeBoolean() => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };

    public override IntegerResult MakeNumber()
    {
        return this;
    }

    public override AbstractResult MakeVariable()
    {
        if (ConstantValue == null)
        {
            return this;
        }
        
        AddCode($"scoreboard players set {MemoryLocation} amethyst {(int)ConstantValue! * DataType.Scale}");
        
        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString(),
            IsTemporary = true,
        };
    }
}