using Antlr4.Runtime;

namespace Amethyst.Model;

public abstract class PreprocessorResult
{
    public abstract PreprocessorDataType DataType { get; }
    public required Compiler Compiler { get; init; }
    public required ParserRuleContext Context { get; init; }

    public abstract bool AsBoolean { get; }
    public abstract int AsInteger { get; }

    public PreprocessorResult Calculate(PreprocessorResult other, ArithmeticOperator op)
    {
        var lhsI = AsInteger;
        var rhsI = other.AsInteger;
        
        double lhsD = lhsI;
        double rhsD = rhsI;
        
        bool isDecimal = false;

        if (this is PreprocessorDecimalResult lhsDecimal)
        {
            lhsD = lhsDecimal.Value;
            isDecimal = true;
        }
        if (other is PreprocessorDecimalResult rhsDecimal)
        {
            rhsD = rhsDecimal.Value;
            isDecimal = true;
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
            
            return new PreprocessorDecimalResult
            {
                Compiler = Compiler,
                Context = Context,
                Value = resultValue
            };
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
            return new PreprocessorIntegerResult
            {
                Compiler = Compiler,
                Context = Context,
                Value = resultValue,
            };
        }
    }
    
    public PreprocessorResult Calculate(PreprocessorResult other, ComparisonOperator op)
    {
        double lhsD = AsInteger;
        double rhsD = other.AsInteger;
        
        if (this is PreprocessorDecimalResult lhsDecimal)
        {
            lhsD = lhsDecimal.Value;
        }
        if (other is PreprocessorDecimalResult rhsDecimal)
        {
            rhsD = rhsDecimal.Value;
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

        return new PreprocessorBooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Value = value
        };
    }
}

public abstract class PreprocessorResult<T> : PreprocessorResult
{
    public required T Value { get; init; }
}