using Amethyst.Utility;

namespace Amethyst.Model;

public enum BasicPreprocessorType
{
    [Description("INT")]
    Int,
    [Description("DEC")]
    Dec,
    [Description("STRING")]
    String,
    [Description("BOOL")]
    Bool,
    [Description("RESOURCE")]
    Resource
}

public class PreprocessorDataType
{
    public required BasicPreprocessorType BasicType { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not PreprocessorDataType type)
        {
            return false;
        }

        return BasicType == type.BasicType;
    }
    
    public override int GetHashCode()
    {
        return BasicType.GetHashCode();
    }

    public override string ToString()
    {
        return BasicType.GetDescription();
    }
    
    public static bool operator ==(PreprocessorDataType? left, PreprocessorDataType? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }
    
    public static bool operator !=(PreprocessorDataType? left, PreprocessorDataType? right)
    {
        if (left is null)
        {
            return right is not null;
        }
        return !left.Equals(right);
    }
}