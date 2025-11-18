using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract class AbstractString : AbstractValue
{
    public override StringDatatype Datatype => new();

    static AbstractString()
    {
        OperationRegistry.Register<AbstractValue, AbstractString, AbstractString>(ArithmeticOperator.ADD, Concatenate);
        OperationRegistry.Register<AbstractString, AbstractValue, AbstractString>(ArithmeticOperator.ADD, Concatenate);
    }
    
    private static AbstractString Concatenate(Compiler compiler, ParserRuleContext context, AbstractValue lhs, AbstractString rhs)
    {
        if (TryCalculateConstants(compiler, context, lhs, rhs, out var result))
        {
            return result;
        }
        
        throw new InvalidOperationException($"Cannot concatenate values of types '{lhs.Datatype}' and '{rhs.Datatype}'.");
    }
    
    private static AbstractString Concatenate(Compiler compiler, ParserRuleContext context, AbstractString lhs, AbstractValue rhs)
    {
        if (TryCalculateConstants(compiler, context, lhs, rhs, out var result))
        {
            return result;
        }
        
        throw new InvalidOperationException($"Cannot concatenate values of types '{lhs.Datatype}' and '{rhs.Datatype}'.");
    }

    private static bool TryCalculateConstants(Compiler compiler, ParserRuleContext context, AbstractValue lhs, AbstractValue rhs, [NotNullWhen(true)] out ConstantString? result)
    {
        result = null;
        
        if (lhs is not IConstantValue lhsConstant || rhs is not IConstantValue rhsConstant)
        {
            return false;
        }

        result = new ConstantString
        {
            Compiler = compiler,
            Context = context,
            Value = lhsConstant.AsString + rhsConstant.AsString
        };

        return true;
    }
}