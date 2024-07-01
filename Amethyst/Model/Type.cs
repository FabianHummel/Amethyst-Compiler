using System.ComponentModel;
using Amethyst.Utility;

namespace Amethyst.Model;

public enum BasicType
{
    [Description("int")]
    Int,
    [Description("dec")]
    Dec,
    [Description("string")]
    String,
    [Description("bool")]
    Bool,
    [Description("array")]
    Array,
    [Description("object")]
    Object,
}

public enum Modifier
{
    [Description("[]")]
    Array,
    [Description("{}")]
    Object
}

public class Type
{
    public required BasicType BasicType { get; init; }
    public Modifier? Modifier { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not Type type)
        {
            return false;
        }

        return BasicType == type.BasicType && Modifier == type.Modifier;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(BasicType, Modifier);
    }

    public override string ToString()
    {
        return $"{BasicType.GetDescription()}{Modifier?.GetDescription()}";
    }

    public bool IsScoreboardType => Modifier == null && BasicType is BasicType.Bool or BasicType.Int or BasicType.Dec;
    
    public bool IsStorageType => Modifier != null || BasicType is BasicType.String or BasicType.Array or BasicType.Object;
}