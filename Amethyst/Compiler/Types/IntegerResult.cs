using Amethyst.Model;

namespace Amethyst;

public class IntegerResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    protected override double AsDecimal => (int)ConstantValue!;

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

    protected override AbstractResult MakeVariable()
    {
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