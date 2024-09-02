using Amethyst.Model;

namespace Amethyst;

public class BooleanResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool,
        Modifier = null
    };

    protected override double AsDecimal => (bool)ConstantValue! ? 1 : 0;

    public override BooleanResult MakeBoolean()
    {
        return this;
    }

    public override IntegerResult MakeNumber()
    {
        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = Location,
            IsTemporary = IsTemporary,
            ConstantValue = ConstantValue,
            Substitutions = Substitutions
        };
    }

    public override AbstractResult MakeVariable()
    {
        if (Location != null)
        {
            return this;
        }
        
        AddCode($"scoreboard players set {MemoryLocation} amethyst {AsDecimal}");
        
        return new BooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString(),
            IsTemporary = true,
        };
    }
}