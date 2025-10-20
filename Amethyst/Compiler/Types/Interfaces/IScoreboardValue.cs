using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Amethyst;

public interface IScoreboardValue : IConstantValue
{
    public int ScoreboardValue { get; }

    static bool TryParse(object value, [NotNullWhen(true)] out IScoreboardValue? result)
    {
        if (value is int intValue)
        {
            result = new ConstantInteger
            {
                Context = null!,
                Compiler = null!,
                Value = intValue
            };
            return true;
        }
        
        if (value is bool boolValue)
        {
            result = new ConstantBoolean
            {
                Context = null!,
                Compiler = null!,
                Value = boolValue
            };
            return true;
        }
        
        if (value is double doubleValue)
        {
            var invariantDoubleString = doubleValue.ToString(CultureInfo.InvariantCulture);
            
            var decimalPlaces = 0;
            if (invariantDoubleString.Contains('.'))
            {
                decimalPlaces = invariantDoubleString.Split('.').Last().Length;
            }
            
            result = new ConstantDecimal
            {
                Context = null!,
                Compiler = null!,
                Value = doubleValue,
                DecimalPlaces = decimalPlaces
            };
            return true;
        }
        
        result = null;
        return false;
    }
}