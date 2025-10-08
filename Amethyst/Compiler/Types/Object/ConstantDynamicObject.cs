using Amethyst.Model;

namespace Amethyst;

public class ConstantDynamicObject : AbstractConstantObject, IMemberAccess
{
    public override AbstractDatatype Datatype => new ObjectDatatype();

    public override AbstractRuntimeObject ToRuntimeValue()
    {
        var location = Location.Storage(++Compiler.StackPointer);

        Compiler.AddCode($"data modify storage {location} set value {ToNbtString()}");

        SubstituteRecursively(Compiler, location);

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