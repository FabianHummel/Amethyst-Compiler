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

    public override IntegerResult MakeNumber()
    {
        return this;
    }

    public override AbstractResult MakeVariable()
    {
        if (Location != null)
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