using Amethyst.Model;

namespace Amethyst;

public class ConstantDynamicObject : AbstractConstantObject, IMemberAccess
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };

    public override AbstractRuntimeObject ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;

        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");

        SubstituteRecursively(Compiler, location.ToString());

        return new RuntimeDynamicObject
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public new AbstractValue? GetMember(string memberName) => memberName switch
    {
        "values" => new ConstantDynamicArray
        {
            Compiler = Compiler,
            Context = Context,
            Value = Value.Values.ToArray()
        },
        _ => base.GetMember(memberName)
    };
}