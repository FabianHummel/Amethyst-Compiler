using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class StaticObjectConstant : ObjectConstantBase
{
    public required BasicType BasicType { get; init; }

    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };
    
    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location.ToString());
        
        return new StaticObjectResult
        {
            BasicType = BasicType,
            Location = location.ToString(),
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public override string ToNbtString()
    {
        return $"{{keys:[{string.Join(",", Value.Keys.Select(key => key.ToNbtString()))}]," +
               $"data:{{{string.Join(',', Value.Select(kvp => $"{kvp.Key.ToNbtString()}:{kvp.Value.ToNbtString()}"))}}}}}";
    }
}