using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Amethyst;

public interface IConstantValue : IEquatable<IConstantValue>
{
    int AsInteger { get; }

    bool AsBoolean { get; }

    double AsDouble { get; }

    /// <summary>Converts this constant value to a string that can be used in NBT data.</summary>
    /// <returns>The NBT string representation of this constant value.</returns>
    string ToNbtString();

    /// <summary>Converts this result to a string that can be used to display the value in Minecraft.</summary>
    /// <returns>A JSON text component that can be used with '/tellraw'.</returns>
    string ToTextComponent();

    /// <summary>Converts this constant value into a string representation that can be used in target
    /// selectors.</summary>
    /// <returns>The target selector's string value representation.</returns>
    string ToTargetSelectorString();

    /// <summary>Converts a constant value to a variable by assigning it a fixed memory location.</summary>
    /// <returns>The result with a place in memory.</returns>
    IRuntimeValue ToRuntimeValue();

    public static bool TryParse(object value, [NotNullWhen(true)] out IConstantValue? result)
    {
        if (IScoreboardValue.TryParse(value, out var scoreboardValue))
        {
            result = scoreboardValue;
            return true;
        }

        if (value is string stringValue)
        {
            result = new ConstantString
            {
                Context = null!,
                Compiler = null!,
                Value = stringValue
            };
            return true;
        }

        if (value is IEnumerable<object> enumerable && AbstractConstantArray.TryParse(enumerable, out var arrayResult))
        {
            result = arrayResult;
            return true;
        }
        
        if (value is IDictionary<string, object> dictionary && AbstractConstantObject.TryParse(dictionary, out var objectResult))
        {
            result = objectResult;
            return true;
        }
        
        result = null;
        return false;
    }
}

public interface IConstantValue<T> : IConstantValue
{
    /// <summary>The constant value of the result that can be directly incorporated into another result
    /// without storing it in a variable. Literals are always stored as constants first and only turn into
    /// variables if they are assigned to a variable.</summary>
    [UsedImplicitly(Reason = "For type safety and clarity.")]
    T Value { get; init; }
}