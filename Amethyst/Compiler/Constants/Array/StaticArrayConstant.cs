using Amethyst.Model;

namespace Amethyst;

public class StaticArrayConstant : ArrayConstantBase
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Array
    };
    
    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location.ToString());
        
        return new StaticArrayResult
        {
            BasicType = BasicType,
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
}