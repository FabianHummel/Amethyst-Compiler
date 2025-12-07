using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class ConstantString : AbstractString, IConstantValue<string>, IMemberAccess
{
    public required string Value { get; init; }
    
    public int AsInteger => Value.Length;
    
    public bool AsBoolean => !string.IsNullOrEmpty(Value);

    public double AsDouble => AsInteger;
    
    public string AsString => Value;

    public IRuntimeValue ToRuntimeValue()
    {
        var location = Location.Storage(++Compiler.StackPointer);
        
        this.AddCode($"data modify storage {location} set value {ToNbtString()}");
        
        return new RuntimeString
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }

    public string ToNbtString()
    {
        return Value.ToNbtString();
    }

    public string ToTextComponent()
    {
        return $$"""["\"",{"text":{{ToNbtString()}},"color":"green"},"\""]""";
    }

    public bool Equals(IConstantValue? other)
    {
        if (other is not ConstantString stringConstant)
        {
            return false;
        }
        
        return Value.Equals(stringConstant.Value);
    }

    public override string ToTargetSelectorString()
    {
        return Value; // TODO: if no spaces or special characters, return as is, else wrap with quotes, or if it contains double quotes, wrap with single quotes, etc...
    }

    public AbstractValue? GetMember(string memberName) => memberName switch
    {
        "length" => new ConstantInteger
        {
            Compiler = Compiler,
            Context = Context,
            Value = Value.Length
        },
        _ => null
    };
}