using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class DynObjectConstant : ObjectConstantBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };
    
    public override RuntimeValue ToRuntimeValue()
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

    public override string ToNbtString()
    {
        return $"{{keys:[{string.Join(",", Value.Keys.Select(key => key.ToNbtString()))}]," +
               $"data:{{{string.Join(',', Value.Select(kvp => $"{kvp.Key.ToNbtString()}:{kvp.Value.ToNbtString()}"))}}}}}";
    }
    
    public override string ToTextComponent()
    {
        var content = string.Join(""",[", "],""", Value.Select(kvp => 
            $$"""{"text":"{{kvp.Key}}","color":"aqua"},[": "],{{kvp.Value.ToTextComponent()}}"""));
        return $$"""["{",{{content}},"}"]""";
    }
}