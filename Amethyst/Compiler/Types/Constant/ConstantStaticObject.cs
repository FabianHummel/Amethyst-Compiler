using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public class ConstantStaticObject : AbstractConstantObject, IMemberAccess, IStaticCollection
{
    public required BasicType BasicType { get; init; }

    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };
    
    public override AbstractRuntimeObject ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location.ToString());
        
        return new RuntimeStaticObject
        {
            BasicType = BasicType,
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
    
    public new AbstractValue GetMember(string memberName)
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