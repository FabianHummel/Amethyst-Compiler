using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class StringConstant : ConstantValue<string>, IMemberAccess
{
    public override int AsInteger => Value.Length;
    
    public override bool AsBoolean => !string.IsNullOrEmpty(Value);
    
    public override DataType DataType => new()
    {
        BasicType = BasicType.String,
        Modifier = null
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        return new StringResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }

    public override string ToNbtString()
    {
        return Value.ToNbtString();
    }

    public override string ToTextComponent()
    {
        return $$"""["\"",{"text":{{ToNbtString()}},"color":"green"},"\""]""";
    }

    public override bool Equals(ConstantValue? other)
    {
        if (other is not StringConstant stringConstant)
        {
            return false;
        }
        
        return Value.Equals(stringConstant.Value);
    }

    public override string ToTargetSelectorString()
    {
        return Value;
    }

    public AbstractResult GetMember(string memberName, AmethystParser.IdentifierContext identifierContext)
    {
        return memberName switch
        {
            "length" => new IntegerConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = Value.Length
            },
            _ => throw new SyntaxException($"Member '{memberName}' not found in string.", identifierContext)
        };
    }
}