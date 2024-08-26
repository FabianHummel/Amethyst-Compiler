using Amethyst.Model;

namespace Amethyst;

public class BooleanResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool,
        Modifier = null
    };

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
            Location = Location
        };
    }

    public override AbstractResult MakeVariable()
    {
        AddCode($"scoreboard players set {MemoryLocation} amethyst {((bool)ConstantValue! ? "1" : "0")}");
        
        return new BooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString(),
            IsTemporary = true,
        };
    }
}