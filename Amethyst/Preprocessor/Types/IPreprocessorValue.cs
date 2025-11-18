using Antlr4.Runtime;
using JetBrains.Annotations;

namespace Amethyst;

/// <summary>A preprocessor value of a specific type. Implemented by all specific preprocessor value
/// types.</summary>
/// <typeparam name="T">The type of the preprocessor value.</typeparam>
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

/// <summary>The non-generic base interface for all preprocessor values. Only implemented by
/// <see cref="AbstractPreprocessorValue" /> to improve the development experience.</summary>
public interface IPreprocessorValue
{
    ParserRuleContext Context { get; }
    
    [UsedImplicitly]
    object? AbstractValue { get; }
    
    [UsedImplicitly]
    void SetValue(object? value);
}