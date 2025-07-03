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

    public override IntegerResult MakeInteger()
    {
        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = Location,
            IsTemporary = IsTemporary
        };
    }
}