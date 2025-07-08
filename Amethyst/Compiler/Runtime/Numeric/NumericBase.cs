using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;


public abstract partial class NumericBase : RuntimeValue
{
    public abstract override DataType DataType { get; }

    private (int, int, int) ApplyScaling(NumericBase lhs, NumericBase rhs)
    {
        var lhsLocation = lhs.Location;
        var rhsLocation = rhs.Location;
        int highestDecimalPlaces = 0;

        if (lhs is DecimalResult || rhs is DecimalResult)
        {
            // 0.01 + 1 -> # decimal places = +2
            // 1 + 0.01 -> # decimal places = -2
            // 0.01 + 0.01 -> # decimal places = 0

            var weightedDecimalPlaces = 0;
            
            if (lhs is DecimalResult lhsDecimal)
            {
                weightedDecimalPlaces += lhsDecimal.DecimalPlaces;
                highestDecimalPlaces = Math.Max(highestDecimalPlaces, lhsDecimal.DecimalPlaces);
            }
            if (rhs is DecimalResult rhsDecimal)
            {
                weightedDecimalPlaces -= rhsDecimal.DecimalPlaces;
                highestDecimalPlaces = Math.Max(highestDecimalPlaces, rhsDecimal.DecimalPlaces);
            }

            var scale = (int)Math.Pow(10, Math.Abs(weightedDecimalPlaces));

            if (weightedDecimalPlaces > 0)
            {
                var rhsValueBackup = rhs.EnsureBackedUp();
                AddCode($"scoreboard players operation {rhsValueBackup.Location} amethyst *= .{scale} amethyst_const");
                rhsLocation = rhsValueBackup.Location;
            }
            if (weightedDecimalPlaces < 0)
            {
                var lhsValueBackup = lhs.EnsureBackedUp();
                AddCode($"scoreboard players operation {lhsValueBackup.Location} amethyst *= .{scale} amethyst_const");
                lhsLocation = lhsValueBackup.Location;
            }
        }
        
        return (lhsLocation, rhsLocation, highestDecimalPlaces);
    }
    
    private NumericBase Calculate(NumericBase rhs, ArithmeticOperator op)
    {
        var lhs = (NumericBase)EnsureBackedUp();
        
        var isDecimal = this is DecimalResult || rhs is DecimalResult;
        
        var (lhsLocation, rhsLocation, highestDecimalPlaces) = ApplyScaling(lhs, rhs);
        
        AddCode($"scoreboard players operation {lhsLocation} amethyst {op.GetMcfOperatorSymbol()}= {rhsLocation} amethyst");

        if (isDecimal)
        {
            return new DecimalResult
            {
                Compiler = Compiler,
                Context = Context,
                Location = lhsLocation,
                IsTemporary = true,
                DecimalPlaces = highestDecimalPlaces
            };
        }
        
        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = lhsLocation,
            IsTemporary = true
        };
    }

    private BooleanResult Calculate(NumericBase rhs, ComparisonOperator op)
    {
        var (lhsLocation, rhsLocation, _) = ApplyScaling(this, rhs);
        
        AddCode($"execute store success score {lhsLocation} amethyst if score {lhsLocation} amethyst {op.GetMcfOperatorSymbol()} {rhsLocation} amethyst");
        
        return new BooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = lhsLocation,
            IsTemporary = true
        };
    }
    
    public override BooleanResult MakeBoolean() => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location,
        IsTemporary = IsTemporary
    };
}