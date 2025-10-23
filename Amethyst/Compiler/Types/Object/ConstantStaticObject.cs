using Amethyst.Model;

namespace Amethyst;

public class ConstantStaticObject : AbstractConstantObject, IMemberAccess
{
    public required BasicType BasicType { get; init; }

    public override AbstractDatatype Datatype => AbstractDatatype.Parse(BasicType, Modifier.Object);
    
    public override AbstractRuntimeObject ToRuntimeValue()
    {
        var location = Location.Storage(++Compiler.StackPointer);
        
        this.AddCode($"data modify storage {location} set value {ToNbtString()}");
        
        SubstituteRecursively(location);
        
        return new RuntimeStaticObject
        {
            BasicType = BasicType,
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
    
    public new AbstractValue? GetMember(string memberName)
    {
        return memberName switch
        {
            "values" => new ConstantStaticArray
            {
                Compiler = Compiler,
                Context = Context,
                BasicType = BasicType,
                Value = Value.Values.ToArray()
            },
            _ => base.GetMember(memberName)
        };
    }
}