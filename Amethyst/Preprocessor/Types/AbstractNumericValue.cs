using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;

namespace Amethyst;

public abstract partial class AbstractNumericPreprocessorValue : AbstractPreprocessorValue
{
    private AbstractNumericPreprocessorValue Calculate(AbstractNumericPreprocessorValue lhs, AbstractNumericPreprocessorValue rhs, ArithmeticOperator op)
    {
        if (TryConvertToDecimals(lhs, rhs, out var decimalLhs, out var decimalRhs))
        {
            var decimalValue = op switch
            {
                ArithmeticOperator.ADD => decimalLhs.Value + decimalRhs.Value,
                ArithmeticOperator.SUBTRACT => decimalLhs.Value - decimalRhs.Value,
                ArithmeticOperator.MULTIPLY => decimalLhs.Value * decimalRhs.Value,
                ArithmeticOperator.DIVIDE => decimalLhs.Value / decimalRhs.Value,
                ArithmeticOperator.MODULO => decimalLhs.Value % decimalRhs.Value,
                _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
            };
            
            return new PreprocessorDecimal
            {
                Compiler = Compiler,
                Context = Context,
                Value = decimalValue
            };
        }

        var integerValue = op switch
        {
            ArithmeticOperator.ADD => lhs.AsInteger + rhs.AsInteger,
            ArithmeticOperator.SUBTRACT => lhs.AsInteger - rhs.AsInteger,
            ArithmeticOperator.MULTIPLY => lhs.AsInteger * rhs.AsInteger,
            ArithmeticOperator.DIVIDE => lhs.AsInteger / rhs.AsInteger,
            ArithmeticOperator.MODULO => lhs.AsInteger % rhs.AsInteger,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
        };
        
        return new PreprocessorInteger
        {
            Compiler = Compiler,
            Context = Context,
            Value = integerValue
        };
    }
    
    private PreprocessorBoolean Calculate(AbstractNumericPreprocessorValue lhs, AbstractNumericPreprocessorValue rhs, ComparisonOperator op)
    {
        bool result;
        if (TryConvertToDecimals(lhs, rhs, out var decimalLhs, out var decimalRhs))
        {
            result = op switch
            {
                ComparisonOperator.LESS_THAN => decimalLhs.Value < decimalRhs.Value,
                ComparisonOperator.LESS_THAN_OR_EQUAL => decimalLhs.Value <= decimalRhs.Value,
                ComparisonOperator.GREATER_THAN => decimalLhs.Value > decimalRhs.Value,
                ComparisonOperator.GREATER_THAN_OR_EQUAL => decimalLhs.Value >= decimalRhs.Value,
                _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
            };
        }
        else
        {
            result = op switch
            {
                ComparisonOperator.LESS_THAN => lhs.AsInteger < rhs.AsInteger,
                ComparisonOperator.LESS_THAN_OR_EQUAL => lhs.AsInteger <= rhs.AsInteger,
                ComparisonOperator.GREATER_THAN => lhs.AsInteger > rhs.AsInteger,
                ComparisonOperator.GREATER_THAN_OR_EQUAL => lhs.AsInteger >= rhs.AsInteger,
                _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
            };
        }

        return new PreprocessorBoolean
        {
            Compiler = Compiler,
            Context = Context,
            Value = result
        };
    }
    
    /// <summary>
    /// Tries to convert both numeric values to decimals if at least one of them is a decimal.
    /// </summary>
    private static bool TryConvertToDecimals(AbstractNumericPreprocessorValue anvLhs, AbstractNumericPreprocessorValue anvRhs, [NotNullWhen(true)] out double? lhs, [NotNullWhen(true)] out double? rhs)
    {
        if (anvLhs is not PreprocessorDecimal && anvRhs is not PreprocessorDecimal)
        {
            lhs = null;
            rhs = null;
            return false;
        }

        lhs = anvLhs.AsDecimal;
        rhs = anvRhs.AsDecimal;
        return true;
    }
}