using System.Diagnostics;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractValue
{
    public required Compiler Compiler { get; init; }
    
    public required ParserRuleContext Context { get; init; }
    
    /// <summary>
    /// The type of the underlying data.
    /// </summary>
    public abstract DataType DataType { get; }
    
    public abstract AbstractString ToStringValue();

    protected void AddCode(string code)
    {
        Compiler.AddCode(code);
    }
    
    protected void AddInitCode(string code)
    {
        Compiler.AddInitCode(code);
    }

    public AbstractValue ToBoolean()
    {
        if (this is IConstantValue constantValue)
        {
            return new ConstantBoolean
            {
                Compiler = Compiler,
                Context = Context,
                Value = constantValue.AsBoolean
            };
        }
        if (this is IRuntimeValue runtimeValue)
        {
            return runtimeValue.MakeBoolean();
        }
        
        throw new UnreachableException($"Cannot convert {GetType().Name} to boolean.");
    }
    
    public static (IRuntimeValue, IConstantValue) EnsureConstantValueIsLast(AbstractValue lhs, AbstractValue rhs)
    {
        // Ensure that the constant value is always the second operand
        if (lhs is IConstantValue constantValue && rhs is IRuntimeValue runtimeValue)
        {
            return (runtimeValue, constantValue);
        }
        if (lhs is IRuntimeValue runtimeValue2 && rhs is IConstantValue constantValue2)
        {
            return (runtimeValue2, constantValue2);
        }

        throw new ArgumentException("One operand must be a constant value and the other must be a runtime value.", nameof(rhs));
    }

    public abstract string ToTargetSelectorString();

    public IRuntimeValue EnsureRuntimeValue()
    {
        if (this is IRuntimeValue runtimeValue)
        {
            return runtimeValue;
        }
        if (this is IConstantValue constantValue)
        {
            return constantValue.ToRuntimeValue();
        }
        
        throw new UnreachableException();
    }

    public AbstractValue Clone()
    {
        return (AbstractValue)MemberwiseClone();
    }
}