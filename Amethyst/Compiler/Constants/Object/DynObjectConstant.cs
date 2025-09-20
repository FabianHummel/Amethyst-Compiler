using Amethyst.Model;

namespace Amethyst;

public class DynObjectConstant : ObjectConstantBase, IMemberAccess
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };

    public override ObjectBase ToRuntimeValue()
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

    public new AbstractResult GetMember(string memberName)
    {
        return memberName switch
        {
            "values" => new DynArrayConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = Value.Values.ToArray()
            },
            _ => base.GetMember(memberName)
        };
    }
}