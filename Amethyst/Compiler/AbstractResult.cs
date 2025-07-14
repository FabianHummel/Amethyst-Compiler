using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract class AbstractResult
{
    public required Compiler Compiler { get; init; }
    
    public required ParserRuleContext Context { get; init; }
    
    /// <summary>
    /// The type of the underlying data.
    /// </summary>
    public abstract DataType DataType { get; }
    
    /// <summary>
    /// Converts a constant value to a variable by assigning it a fixed memory location.
    /// </summary>
    /// <returns>The result with a place in memory.</returns>
    public abstract RuntimeValue ToRuntimeValue();
    
    public bool TryCalculateConstants(AbstractResult other, ArithmeticOperator op, [NotNullWhen(true)] out ConstantValue? result)
    {
        result = null;
        
        if (this is not ConstantValue lhs || other is not ConstantValue rhs)
        {
            return false;
        }
        
        var lhsI = lhs.AsInteger;
        var rhsI = rhs.AsInteger;
        
        double lhsD = lhsI;
        double rhsD = rhsI;
        
        bool isDecimal = false;
        int maxDecimalPlaces = 0;

        if (lhs is DecimalConstant lhsDecimal)
        {
            lhsD = lhsDecimal.AsDouble;
            isDecimal = true;
            maxDecimalPlaces = lhsDecimal.DecimalPlaces;
        }
        if (rhs is DecimalConstant rhsDecimal)
        {
            rhsD = rhsDecimal.AsDouble;
            isDecimal = true;
            maxDecimalPlaces = Math.Max(maxDecimalPlaces, rhsDecimal.DecimalPlaces);
        }
        
        // If any of the operands is a decimal, we also need to return a decimal result
        if (isDecimal)
        {
            var resultValue = op switch
            {
                ArithmeticOperator.ADD => lhsD + rhsD,
                ArithmeticOperator.SUBTRACT => lhsD - rhsD,
                ArithmeticOperator.MULTIPLY => lhsD * rhsD,
                ArithmeticOperator.DIVIDE => lhsD / rhsD,
                ArithmeticOperator.MODULO => lhsD % rhsD,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
            
            result = new DecimalConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = resultValue,
                DecimalPlaces = maxDecimalPlaces
            };
            
            return true;
        }

        else
        {
            var resultValue = op switch
            {
                ArithmeticOperator.ADD => lhsI + rhsI,
                ArithmeticOperator.SUBTRACT => lhsI - rhsI,
                ArithmeticOperator.MULTIPLY => lhsI * rhsI,
                ArithmeticOperator.DIVIDE => lhsI / rhsI,
                ArithmeticOperator.MODULO => lhsI % rhsI,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
            
            // Otherwise, we return an integer result
            result = new IntegerConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = resultValue,
            };
        
            return true;
        }
    }
    
    public bool TryCalculateConstants(AbstractResult other, ComparisonOperator op, [NotNullWhen(true)] out ConstantValue? result)
    {
        result = null;

        if (this is not ConstantValue lhs || other is not ConstantValue rhs)
        {
            return false;
        }

        double lhsD = lhs.AsInteger;
        double rhsD = rhs.AsInteger;
        
        if (lhs is DecimalConstant lhsDecimal)
        {
            lhsD = lhsDecimal.AsDouble;
        }
        if (rhs is DecimalConstant rhsDecimal)
        {
            rhsD = rhsDecimal.AsDouble;
        }
        
        int comparison = lhsD.CompareTo(rhsD);
        
        bool value = op switch
        {
            ComparisonOperator.EQUAL => comparison == 0,
            ComparisonOperator.NOT_EQUAL => comparison != 0,
            ComparisonOperator.LESS_THAN => comparison < 0,
            ComparisonOperator.LESS_THAN_OR_EQUAL => comparison <= 0,
            ComparisonOperator.GREATER_THAN => comparison > 0,
            ComparisonOperator.GREATER_THAN_OR_EQUAL => comparison >= 0,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };

        result = new BooleanConstant
        {
            Compiler = Compiler,
            Context = Context,
            Value = value
        };

        return true;
    }

    public AbstractResult ToBoolean()
    {
        if (this is ConstantValue constantValue)
        {
            return new BooleanConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = constantValue.AsBoolean
            };
        }
        if (this is RuntimeValue runtimeValue)
        {
            return runtimeValue.MakeBoolean();
        }
        
        throw new UnreachableException($"Cannot convert {GetType().Name} to boolean.");
    }
    
    public static (RuntimeValue, ConstantValue) EnsureConstantValueIsLast(AbstractResult lhs, AbstractResult rhs)
    {
        // Ensure that the constant value is always the second operand
        if (lhs is ConstantValue constantValue && rhs is RuntimeValue runtimeValue)
        {
            return (runtimeValue, constantValue);
        }

        if (lhs is RuntimeValue runtimeValue2 && rhs is ConstantValue constantValue2)
        {
            return (runtimeValue2, constantValue2);
        }

        throw new ArgumentException("One operand must be a constant value and the other must be a runtime value.", nameof(rhs));
    }
}