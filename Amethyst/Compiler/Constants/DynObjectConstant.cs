using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class DynObjectConstant : ConstantValue<Dictionary<string, ConstantValue>>
{
    public override int AsInteger => Value.Count;
    
    public override bool AsBoolean => AsInteger > 0;

    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };
    
    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        return new DynObjectResult
        {
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