using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public class StaticObjectConstant : ObjectConstantBase, IMemberAccess
{
    public required BasicType BasicType { get; init; }

    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };
    
    public override ObjectBase ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location.ToString());
        
        return new StaticObjectResult
        {
            BasicType = BasicType,
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
    
    public new AbstractResult GetMember(string memberName, AmethystParser.IdentifierContext identifierContext)
    {
        return memberName switch
        {
            "values" => new StaticArrayConstant
            {
                Compiler = Compiler,
                Context = Context,
                BasicType = BasicType,
                Value = Value.Values.ToArray()
            },
            _ => base.GetMember(memberName, identifierContext)
        };
    }
}