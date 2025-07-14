using Amethyst.Model;

namespace Amethyst;

public class DynObjectConstant : ObjectConstantBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;

        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");

        SubstituteRecursively(Compiler, location.ToString());

        return new DynObjectResult
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
}