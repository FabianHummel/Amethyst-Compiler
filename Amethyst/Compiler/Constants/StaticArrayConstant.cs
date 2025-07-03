using Amethyst.Model;

namespace Amethyst;

public class StaticArrayConstant : ConstantValue<ConstantValue[]>
{
    public required BasicType BasicType { get; init; }
    
    public override int AsInteger => Value.Length;
    
    public override bool AsBoolean => AsInteger > 0;

    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Array
    };
    
    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        return new StaticArrayResult
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
        return $"[{string.Join(",", Value.Select(v => v.ToNbtString()))}]";
    }
}