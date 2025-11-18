using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractNumericPreprocessorValue : AbstractPreprocessorValue
{
    static AbstractNumericPreprocessorValue()
    {
        OperationRegistry.Register<AbstractNumericPreprocessorValue, AbstractNumericPreprocessorValue, AbstractNumericPreprocessorValue, ArithmeticOperator>(
            Calculate);
        OperationRegistry.Register<AbstractNumericPreprocessorValue, AbstractNumericPreprocessorValue, PreprocessorBoolean, ComparisonOperator>(
            Calculate);
    }
    
    /// <summary>Calculates the result of two numeric values using the specified arithmetic operator.</summary>
    /// <param name="lhs">A numeric preprocessor value on the left side of the operation.</param>
    /// <param name="rhs">A numeric preprocessor value on the right side of the operation.</param>
    /// <param name="op">The arithmetic operator to use for the calculation.</param>
    /// <returns>The resulting numeric preprocessor value after applying the operation.</returns>
    /// <exception cref="SyntaxException">Thrown when an invalid operator is provided.</exception>
    /// <seealso
    ///     cref="Calculate(Amethyst.Compiler,Antlr4.Runtime.ParserRuleContext,Amethyst.AbstractNumericPreprocessorValue,Amethyst.AbstractNumericPreprocessorValue,Amethyst.Model.ComparisonOperator" />
    private static AbstractNumericPreprocessorValue Calculate(Compiler compiler, ParserRuleContext context, AbstractNumericPreprocessorValue lhs, AbstractNumericPreprocessorValue rhs, ArithmeticOperator op)
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
                _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
            };
            
            return new PreprocessorDecimal
            {
                Compiler = compiler,
                Context = context,
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
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
        
        return new PreprocessorInteger
        {
            Compiler = compiler,
            Context = context,
            Value = integerValue
        };
    }

    /// <summary>Calculates the result of two numeric values using the specified comparison operator.</summary>
    /// <param name="lhs">A numeric preprocessor value on the left side of the comparison.</param>
    /// <param name="rhs">A numeric preprocessor value on the right side of the comparison.</param>
    /// <param name="op">The comparison operator to use for the calculation.</param>
    /// <returns>The resulting boolean preprocessor value after applying the comparison.</returns>
    /// <exception cref="SyntaxException">Thrown when an invalid operator is provided.</exception>
    /// <seealso
    ///     cref="Calculate(Amethyst.Compiler,Antlr4.Runtime.ParserRuleContext,Amethyst.AbstractNumericPreprocessorValue,Amethyst.AbstractNumericPreprocessorValue,Amethyst.Model.ArithmeticOperator)" />
    private static PreprocessorBoolean Calculate(Compiler compiler, ParserRuleContext context, AbstractNumericPreprocessorValue lhs, AbstractNumericPreprocessorValue rhs, ComparisonOperator op)
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
                _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
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
                _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
            };
        }

        return new PreprocessorBoolean
        {
            Compiler = compiler,
            Context = context,
            Value = result
        };
    }

    /// <summary>Tries to convert both numeric values to decimals if at least one of them is a decimal.</summary>
    /// <param name="anvLhs">The left-hand side numeric preprocessor value.</param>
    /// <param name="anvRhs">The right-hand side numeric preprocessor value.</param>
    /// <param name="lhs">The converted left-hand side decimal value, or null if conversion is not
    /// possible.</param>
    /// <param name="rhs">The converted right-hand side decimal value, or null if conversion is not
    /// possible.</param>
    /// <returns>>True if conversion to decimals was successful; otherwise, false.</returns>
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