using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Amethyst.Utility;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractNumericValue : AbstractValue
{
    protected abstract AbstractDecimal AsDecimal { get; }

    protected abstract AbstractScoreboardDatatype ScoreboardDatatype { get; }

    public override AbstractDatatype Datatype => ScoreboardDatatype;

    static AbstractNumericValue()
    {
        OperationRegistry.Register<AbstractNumericValue, AbstractNumericValue, AbstractNumericValue, ArithmeticOperator>(Calculate);
        OperationRegistry.Register<AbstractNumericValue, AbstractNumericValue, AbstractBoolean, ComparisonOperator>(Calculate);
    }
    
    /// <summary>Calculates any two numeric values with the given operator.</summary>
    private static AbstractNumericValue Calculate(Compiler compiler, ParserRuleContext context, AbstractNumericValue lhs, AbstractNumericValue rhs, ArithmeticOperator op)
    {
        if (TryCalculateConstants(compiler, context, lhs, rhs, op, out var result))
        {
            return result;
        }

        if (TryConvertToDecimals(lhs, rhs, out var decimalLhs, out var decimalRhs))
        {
            var runtimeDecimalLhs = (RuntimeDecimal)decimalLhs.EnsureRuntimeValue();
            var runtimeDecimalRhs = (RuntimeDecimal)decimalRhs.EnsureRuntimeValue();
            var resultScaling = ApplyDecimalScaling(compiler, context, ref runtimeDecimalLhs, ref runtimeDecimalRhs, op);
            var decimalResult = PerformCalculation(compiler, context, runtimeDecimalLhs, runtimeDecimalRhs, op).AsDecimal;
            decimalResult.DecimalPlaces = resultScaling;
            return decimalResult;
        }
        
        var runtimeLhs = lhs.EnsureRuntimeValue();
        var runtimeRhs = rhs.EnsureRuntimeValue();
        return PerformCalculation(compiler, context, runtimeLhs, runtimeRhs, op);
    }

    /// <summary>Tries to calculate the result of two constant numeric values with the given operator.</summary>
    private static bool TryCalculateConstants(Compiler compiler, ParserRuleContext context, AbstractNumericValue anvLhs, AbstractNumericValue anvRhs, ArithmeticOperator op, [NotNullWhen(true)] out AbstractNumericValue? result)
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
                _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
            };
            
            result = new ConstantDecimal
            {
                Compiler = compiler,
                Context = context,
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
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
        
        result = new ConstantInteger
        {
            Compiler = compiler,
            Context = context,
            Value = integerValue
        };
        
        return true;
    }

    /// <summary>Calculates any two numeric values with the given operator.</summary>
    private static AbstractBoolean Calculate(Compiler compiler, ParserRuleContext context, AbstractNumericValue lhs, AbstractNumericValue rhs, ComparisonOperator op)
    {
        if (TryCalculateConstants(context, lhs, rhs, op, out var result))
        {
            return new ConstantBoolean
            {
                Compiler = compiler,
                Context = context,
                Value = result.Value
            };
        }

        if (TryConvertToDecimals(lhs, rhs, out var decimalLhs, out var decimalRhs))
        {
            var runtimeDecimalLhs = (RuntimeDecimal)decimalLhs.EnsureRuntimeValue();
            var runtimeDecimalRhs = (RuntimeDecimal)decimalRhs.EnsureRuntimeValue();
            ApplyDecimalScaling(compiler, context,ref runtimeDecimalLhs, ref runtimeDecimalRhs);
            return PerformCalculation(compiler, context, runtimeDecimalLhs, runtimeDecimalRhs, op);
        }
        
        var runtimeLhs = lhs.EnsureRuntimeValue();
        var runtimeRhs = rhs.EnsureRuntimeValue();
        return PerformCalculation(compiler, context, runtimeLhs, runtimeRhs, op);
    }

    /// <summary>Tries to calculate the result of two constant numeric values with the given operator.</summary>
    private static bool TryCalculateConstants(ParserRuleContext context, AbstractNumericValue anvLhs, AbstractNumericValue anvRhs, ComparisonOperator op, [NotNullWhen(true)] out bool? result)
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
                _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
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
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
        
        return true;
    }

    /// <summary>Tries to convert both numeric values to decimals if at least one of them is a decimal.</summary>
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

    /// <summary>Applies decimal scaling to two runtime decimal values so that they have the same number of
    /// decimal places in order to perform arithmetic operations on them through scoreboards.</summary>
    private static int ApplyDecimalScaling(Compiler compiler, ParserRuleContext context, ref RuntimeDecimal lhs, ref RuntimeDecimal rhs, ArithmeticOperator? op = null)
    {
        switch (op)
        {
            case ArithmeticOperator.ADD or ArithmeticOperator.SUBTRACT or ArithmeticOperator.MODULO:
            {
                var weightedDecimalPlaces = lhs.DecimalPlaces - rhs.DecimalPlaces;
                var highestDecimalPlaces = Math.Max(lhs.DecimalPlaces, rhs.DecimalPlaces);
        
                var scale = (int)Math.Pow(10, Math.Abs(weightedDecimalPlaces));
        
                if (weightedDecimalPlaces < 0)
                {
                    lhs = (RuntimeDecimal)lhs.EnsureBackedUp();
                    lhs.DecimalPlaces = highestDecimalPlaces;
                    compiler.AddCode($"scoreboard players operation {lhs.Location} *= #{scale} amethyst_const");
                    compiler.Namespace.ScoreboardConstants.Add(scale);
                }
        
                if (weightedDecimalPlaces > 0)
                {
                    rhs = (RuntimeDecimal)rhs.EnsureBackedUp();
                    lhs.DecimalPlaces = highestDecimalPlaces;
                    compiler.AddCode($"scoreboard players operation {rhs.Location} *= #{scale} amethyst_const");
                    compiler.Namespace.ScoreboardConstants.Add(scale);
                }

                return highestDecimalPlaces;
            }
            
            case ArithmeticOperator.MULTIPLY:
            {
                return lhs.DecimalPlaces + rhs.DecimalPlaces;
            }
            
            case ArithmeticOperator.DIVIDE:
            {
                if (rhs.DecimalPlaces > lhs.DecimalPlaces)
                {
                    throw new SemanticException($"Right side of division can't have higher precision than left side, because the result would always be zero. ({rhs.DecimalPlaces} > {lhs.DecimalPlaces})", context);
                }
                
                return lhs.DecimalPlaces - rhs.DecimalPlaces;
            }
        }
        
        throw new InvalidOperationException("Decimal scaling can only be applied for arithmetic operations.");
    }

    /// <summary>Performs arithmetic on two runtime values. This expects both values to be already safe for
    /// any calculations.</summary>
    private static RuntimeInteger PerformCalculation(Compiler compiler, ParserRuleContext context, IRuntimeValue lhs, IRuntimeValue rhs, ArithmeticOperator op)
    {
        var runtimeLhsBackup = lhs.EnsureBackedUp();
        compiler.AddCode($"scoreboard players operation {runtimeLhsBackup.Location} {op.GetMcfOperatorSymbol()}= {rhs.Location}");
        return new RuntimeInteger
        {
            Compiler = compiler,
            Context = context,
            Location = runtimeLhsBackup.Location,
            IsTemporary = true
        };
    }

    /// <summary>Performs logical comparison on two runtime values. This expects both values to be already
    /// safe for any comparison.</summary>
    private static RuntimeBoolean PerformCalculation(Compiler compiler, ParserRuleContext context, IRuntimeValue lhs, IRuntimeValue rhs, ComparisonOperator op)
    {
        var location = lhs.NextFreeLocation(DataLocation.Scoreboard);
        compiler.AddCode($"execute store success score {location} if score {lhs.Location} {op.GetMcfOperatorSymbol()} {rhs.Location}");
        return new RuntimeBoolean
        {
            Compiler = compiler,
            Context = context,
            Location = location,
            IsTemporary = true
        };
    }
}