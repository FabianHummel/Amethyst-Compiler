using Amethyst.Model;

namespace Amethyst;

public class DynArrayResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };

    protected override AbstractResult MakeVariable()
    {
        AddCode($"data modify storage amethyst: {MemoryLocation} set value {ConstantValue}");

        SubstituteRecursively(MemoryLocation.ToString());

        return new DynArrayResult
        {
            Compiler = Compiler,
            Location = MemoryLocation++.ToString(),
            Context = Context
        };
    }
}