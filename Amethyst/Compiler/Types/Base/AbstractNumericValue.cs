using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public abstract partial class AbstractNumericValue : AbstractAmethystValue
{
    protected abstract AbstractDecimal AsDecimal { get; }

    protected abstract AbstractScoreboardDatatype ScoreboardDatatype { get; }

    public override AbstractDatatype Datatype => ScoreboardDatatype;

    /// <summary>
    /// Ensures that this number is stored in storage, likely to be able to use it as macro input.
    /// </summary>
    /// <returns>A runtime value pointing to a storage location</returns>
    public IRuntimeValue EnsureInStorage()
    {
        if (this is not IRuntimeValue runtimeValue)
        {
            throw new InvalidOperationException("Call of this method is not allowed on constant values.");
        }
        
        var location = runtimeValue.NextFreeLocation(DataLocation.Storage);
        
        this.AddCode($"execute store result storage {location} {ScoreboardDatatype.StorageModifier} run scoreboard players get {runtimeValue.Location}");

        return ((IRuntimeValue)this).WithLocation(location, temporary: true);
    }

    /// <summary>
    /// Calculates any two numeric values with the given operator.
    /// </summary>
    private AbstractNumericValue Calculate(AbstractNumericValue lhs, AbstractNumericValue rhs, ArithmeticOperator op)
    {
        if (TryCalculateConstants(lhs, rhs, op, out var result))
        {
            return result;
        }

        if (TryConvertToDecimals(lhs, rhs, out var decimalLhs, out var decimalRhs))
        {
            var runtimeDecimalLhs = (RuntimeDecimal)decimalLhs.EnsureRuntimeValue();
            var runtimeDecimalRhs = (RuntimeDecimal)decimalRhs.EnsureRuntimeValue();
            ApplyDecimalScaling(ref runtimeDecimalLhs, ref runtimeDecimalRhs);
            return PerformCalculation(runtimeDecimalLhs, runtimeDecimalRhs, op);
        }
        
        var runtimeLhs = lhs.EnsureRuntimeValue();
        var runtimeRhs = rhs.EnsureRuntimeValue();
        return PerformCalculation(runtimeLhs, runtimeRhs, op);
    }
    
    /// <summary>
    /// Tries to calculate the result of two constant numeric values with the given operator.
    /// </summary>
    private bool TryCalculateConstants(AbstractNumericValue anvLhs, AbstractNumericValue anvRhs, ArithmeticOperator op, [NotNullWhen(true)] out AbstractNumericValue? result)
    {
        result = null;
        if (anvLhs is not IConstantValue lhs || anvRhs is not IConstantValue rhs)
        {
            return false;
        }

        if (TryConvertToDecimals(anvLhs, anvRhs, out var decimalLhs, out var decimalRhs))
        {
            var constantDecimalLhs = (ConstantDecimal)decimalLhs;
            var constantDecimalRhs = (ConstantDecimal)decimalRhs;
            
            var decimalValue = op switch
            {
                ArithmeticOperator.ADD => constantDecimalLhs.Value + constantDecimalRhs.Value,
                ArithmeticOperator.SUBTRACT => constantDecimalLhs.Value - constantDecimalRhs.Value,
                ArithmeticOperator.MULTIPLY => constantDecimalLhs.Value * constantDecimalRhs.Value,
                ArithmeticOperator.DIVIDE => constantDecimalLhs.Value / constantDecimalRhs.Value,
                ArithmeticOperator.MODULO => constantDecimalLhs.Value % constantDecimalRhs.Value,
                _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
            };
            
            result = new ConstantDecimal
            {
                Compiler = Compiler,
                Context = Context,
                DecimalPlaces = Math.Max(constantDecimalLhs.DecimalPlaces, constantDecimalRhs.DecimalPlaces),
                Value = decimalValue
            };
            
            return true;
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
        
        result = new ConstantInteger
        {
            Compiler = Compiler,
            Context = Context,
            Value = integerValue
        };
        
        return true;
    }
    
    /// <summary>
    /// Calculates any two numeric values with the given operator.
    /// </summary>
    private AbstractNumericValue Calculate(AbstractNumericValue lhs, AbstractNumericValue rhs, ComparisonOperator op)
    {
        if (TryCalculateConstants(lhs, rhs, op, out var result))
        {
            return new ConstantBoolean
            {
                Compiler = Compiler,
                Context = Context,
                Value = result.Value
            };
        }

        if (TryConvertToDecimals(lhs, rhs, out var decimalLhs, out var decimalRhs))
        {
            var runtimeDecimalLhs = (RuntimeDecimal)decimalLhs.EnsureRuntimeValue();
            var runtimeDecimalRhs = (RuntimeDecimal)decimalRhs.EnsureRuntimeValue();
            ApplyDecimalScaling(ref runtimeDecimalLhs, ref runtimeDecimalRhs);
            return PerformCalculation(runtimeDecimalLhs, runtimeDecimalRhs, op);
        }
        
        var runtimeLhs = lhs.EnsureRuntimeValue();
        var runtimeRhs = rhs.EnsureRuntimeValue();
        return PerformCalculation(runtimeLhs, runtimeRhs, op);
    }
    
    /// <summary>
    /// Tries to calculate the result of two constant numeric values with the given operator.
    /// </summary>
    private bool TryCalculateConstants(AbstractNumericValue anvLhs, AbstractNumericValue anvRhs, ComparisonOperator op, [NotNullWhen(true)] out bool? result)
    {
        result = null;
        if (anvLhs is not IConstantValue lhs || anvRhs is not IConstantValue rhs)
        {
            return false;
        }

        if (TryConvertToDecimals(anvLhs, anvRhs, out var decimalLhs, out var decimalRhs))
        {
            var constantDecimalLhs = (ConstantDecimal)decimalLhs;
            var constantDecimalRhs = (ConstantDecimal)decimalRhs;
            
            result = op switch
            {
                ComparisonOperator.LESS_THAN => constantDecimalLhs.Value < constantDecimalRhs.Value,
                ComparisonOperator.LESS_THAN_OR_EQUAL => constantDecimalLhs.Value <= constantDecimalRhs.Value,
                ComparisonOperator.GREATER_THAN => constantDecimalLhs.Value > constantDecimalRhs.Value,
                ComparisonOperator.GREATER_THAN_OR_EQUAL => constantDecimalLhs.Value >= constantDecimalRhs.Value,
                ComparisonOperator.EQUAL => Math.Abs(constantDecimalLhs.Value - constantDecimalRhs.Value) < float.Epsilon,
                ComparisonOperator.NOT_EQUAL => Math.Abs(constantDecimalLhs.Value - constantDecimalRhs.Value) >= float.Epsilon,
                _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
            };
            
            return true;
        }

        result = op switch
        {
            ComparisonOperator.LESS_THAN => lhs.AsInteger < rhs.AsInteger,
            ComparisonOperator.LESS_THAN_OR_EQUAL => lhs.AsInteger <= rhs.AsInteger,
            ComparisonOperator.GREATER_THAN => lhs.AsInteger > rhs.AsInteger,
            ComparisonOperator.GREATER_THAN_OR_EQUAL => lhs.AsInteger >= rhs.AsInteger,
            ComparisonOperator.EQUAL => lhs.AsInteger == rhs.AsInteger,
            ComparisonOperator.NOT_EQUAL => lhs.AsInteger != rhs.AsInteger,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", Context)
        };
        
        return true;
    }
    
    /// <summary>
    /// Tries to convert both numeric values to decimals if at least one of them is a decimal.
    /// </summary>
    private static bool TryConvertToDecimals(AbstractNumericValue anvLhs, AbstractNumericValue anvRhs, [NotNullWhen(true)] out AbstractDecimal? lhs, [NotNullWhen(true)] out AbstractDecimal? rhs)
    {
        lhs = null;
        rhs = null;
        if (anvLhs is not AbstractDecimal && anvRhs is not AbstractDecimal)
        {
            return false;
        }

        lhs = anvLhs.AsDecimal;
        rhs = anvRhs.AsDecimal;
        return true;
    }

    /// <summary>
    /// Applies decimal scaling to two runtime decimal values so that they have the same number
    /// of decimal places in order to perform arithmetic operations on them through scoreboards.
    /// </summary>
    private void ApplyDecimalScaling(ref RuntimeDecimal lhs, ref RuntimeDecimal rhs)
    {
        var weightedDecimalPlaces = lhs.DecimalPlaces - rhs.DecimalPlaces;
        var highestDecimalPlaces = Math.Max(lhs.DecimalPlaces, rhs.DecimalPlaces);
        
        var scale = (int)Math.Pow(10, Math.Abs(weightedDecimalPlaces));
        
        if (weightedDecimalPlaces < 0)
        {
            lhs = (RuntimeDecimal)lhs.EnsureBackedUp();
            lhs.DecimalPlaces = highestDecimalPlaces;
            this.AddCode($"scoreboard players operation {lhs.Location} *= .{scale} amethyst_const");
        }
        
        if (weightedDecimalPlaces > 0)
        {
            rhs = (RuntimeDecimal)rhs.EnsureBackedUp();
            lhs.DecimalPlaces = highestDecimalPlaces;
            this.AddCode($"scoreboard players operation {rhs.Location} *= .{scale} amethyst_const");
        }
    }

    /// <summary>
    /// Performs arithmetic on two runtime values.
    /// This expects both values to be already safe for any calculations.
    /// </summary>
    private AbstractNumericValue PerformCalculation(IRuntimeValue lhs, IRuntimeValue rhs, ArithmeticOperator op)
    {
        var runtimeLhsBackup = lhs.EnsureBackedUp();
        this.AddCode($"scoreboard players operation {runtimeLhsBackup.Location} {op.GetMcfOperatorSymbol()}= {rhs.Location}");
        return (AbstractNumericValue)runtimeLhsBackup;
    }
    
    /// <summary>
    /// Performs logical comparison on two runtime values. This
    /// expects both values to be already safe for any comparison.
    /// </summary>
    private RuntimeBoolean PerformCalculation(IRuntimeValue lhs, IRuntimeValue rhs, ComparisonOperator op)
    {
        var location = lhs.NextFreeLocation(DataLocation.Scoreboard);
        this.AddCode($"execute store success score {location} if score {lhs.Location} {op.GetMcfOperatorSymbol()} {rhs.Location}");
        return new RuntimeBoolean
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}