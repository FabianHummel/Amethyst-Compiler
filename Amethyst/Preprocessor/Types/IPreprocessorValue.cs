using Amethyst.Utility;
using Antlr4.Runtime;
using JetBrains.Annotations;

namespace Amethyst;

[ForwardDefaultInterfaceMethods]
public interface IPreprocessorValue<T> : IPreprocessorValue
{
    T Value { get; set; }
    
    public new sealed void SetValue(object? value)
    {
        if (value is not T typedValue)
        {
            throw new SyntaxException("Type mismatch in assignment.", Context);
        }
        
        Value = typedValue;
    }
    
    public new sealed object? AbstractValue => Value;
}

public interface IPreprocessorValue
{
    ParserRuleContext Context { get; }
    
    [UsedImplicitly]
    object? AbstractValue { get; }
    
    [UsedImplicitly]
    void SetValue(object? value);
}