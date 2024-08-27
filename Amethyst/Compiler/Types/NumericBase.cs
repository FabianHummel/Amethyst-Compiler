using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public abstract class NumericBase : AbstractResult
{
    public abstract override DataType DataType { get; }
    
    protected abstract double ConstantValueAsDecimal { get; }
    
    private bool TryCalculateConstants(NumericBase rhs, ArithmeticOperator op, [NotNullWhen(true)] out AbstractResult? result)
    {
        result = null;
        
        if (ConstantValue is null || rhs.ConstantValue is null)
        {
            return false;
        }
        
        var leftValue = ConstantValueAsDecimal;
        var rightValue = rhs.ConstantValueAsDecimal;
        
        var resultValue = op switch
        {
            ArithmeticOperator.ADD => leftValue + rightValue,
            ArithmeticOperator.SUBTRACT => leftValue - rightValue,
            ArithmeticOperator.MULTIPLY => leftValue * rightValue,
            ArithmeticOperator.DIVIDE => leftValue / rightValue,
            ArithmeticOperator.MODULO => leftValue % rightValue,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
        
        // If any of the operands is a decimal, we also need to return a decimal result
        if (this is DecimalResult || rhs is DecimalResult)
        {
            result = new DecimalResult
            {
                Compiler = Compiler,
                Context = Context,
                ConstantValue = (double)resultValue
            };
            
            return true;
        }
        
        // Otherwise, we return an integer result
        result = new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            ConstantValue = (int)resultValue
        };
        
        return true;
    }
    
    private AbstractResult Calculate(NumericBase rhs, ArithmeticOperator op)
    {
        if (TryCalculateConstants(rhs, op, out var result))
        {
            return result;
        }

        var lhsValue = MakeVariable();
        var rhsValue = rhs.MakeVariable();
        
        var isDecimal = this is DecimalResult || rhs is DecimalResult;

        if (lhsValue is not DecimalResult && rhsValue is DecimalResult)
        {
            AddCode($"scoreboard players operation {lhsValue.Location} amethyst *= .{BasicType.Dec.GetScale()} amethyst_const");
        }
        
        if (lhsValue is DecimalResult && rhsValue is not DecimalResult)
        {
            AddCode($"scoreboard players operation {rhsValue.Location} amethyst *= .{BasicType.Dec.GetScale()} amethyst_const");
        }
        
        AddCode($"scoreboard players operation {lhsValue.Location} amethyst {op.GetMcfOperatorSymbol()}= {rhsValue.Location} amethyst");

        if (isDecimal)
        {
            return new DecimalResult
            {
                Compiler = Compiler,
                Context = Context,
                Location = lhsValue.Location,
                IsTemporary = true
            };
        }
        
        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = lhsValue.Location,
            IsTemporary = true
        };
    }

    protected override AbstractResult VisitAdd(IntegerResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.ADD);
    }

    protected override AbstractResult VisitAdd(DecimalResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.ADD);
    }

    protected override AbstractResult VisitAdd(BooleanResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.ADD);
    }

    protected override AbstractResult VisitSubtract(IntegerResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.SUBTRACT);
    }

    protected override AbstractResult VisitSubtract(DecimalResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.SUBTRACT);
    }

    protected override AbstractResult VisitSubtract(BooleanResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.SUBTRACT);
    }

    protected override AbstractResult VisitMultiply(IntegerResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.MULTIPLY);
    }

    protected override AbstractResult VisitMultiply(DecimalResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.MULTIPLY);
    }

    protected override AbstractResult VisitMultiply(BooleanResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.MULTIPLY);
    }

    protected override AbstractResult VisitDivide(IntegerResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.DIVIDE);
    }

    protected override AbstractResult VisitDivide(DecimalResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.DIVIDE);
    }

    protected override AbstractResult VisitDivide(BooleanResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.DIVIDE);
    }

    protected override AbstractResult VisitModulo(IntegerResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.MODULO);
    }

    protected override AbstractResult VisitModulo(DecimalResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.MODULO);
    }

    protected override AbstractResult VisitModulo(BooleanResult rhs)
    {
        return Calculate(rhs, ArithmeticOperator.MODULO);
    }
}