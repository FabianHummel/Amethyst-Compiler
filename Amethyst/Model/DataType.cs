using Amethyst.Model.Attributes;
using Amethyst.Utility;

namespace Amethyst.Model;

public enum BasicType
{
    [Description("int")]
    [DefaultValue("0")]
    Int,
    [Description("dec")]
    [DefaultValue("0d")]
    Dec,
    [Description("string")]
    [DefaultValue("\"\"")]
    String,
    [Description("bool")]
    [DefaultValue("0b")]
    Bool,
    [Description("array")]
    [DefaultValue("[]")]
    [SubstitutionModifier("[{0}]")]
    Array,
    [Description("object")]
    [DefaultValue("{}")]
    [SubstitutionModifier(".data.{0}")]
    Object,
    [Description("unknown")]
    Unknown
}

public enum Modifier
{
    [Description("[]")]
    [DefaultValue("[]")]
    [SubstitutionModifier("[{0}]")]
    Array,
    [Description("{}")]
    [DefaultValue("{}")]
    [SubstitutionModifier(".data.{0}")]
    Object
}

public class DataType
{
    public required BasicType BasicType { get; init; }
    public Modifier? Modifier { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not DataType type)
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
    
    public static bool operator ==(DataType? left, DataType? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }
    
    public static bool operator !=(DataType? left, DataType? right)
    {
        if (left is null)
        {
            return right is not null;
        }
        return !left.Equals(right);
    }
    
    public DataLocation Location => Modifier == null && BasicType is BasicType.Bool or BasicType.Int or BasicType.Dec 
        ? DataLocation.Scoreboard
        : DataLocation.Storage;
    
    // TODO: Refactor this to use a more structured approach for storage modifiers
    public string? StorageModifier
    {
        get
        {
            if (Location == DataLocation.Storage)
            {
                return null;
            }
        
            if (BasicType == BasicType.Bool)
            {
                return "byte 1";
            }
            if (BasicType == BasicType.Int)
            {
                return "int 1";
            }
            if (this is DecimalDataType decimalDataType)
            {
                return $"double {1.0 / decimalDataType.Scale}";
            }
        
            return null;
        }
    }

    public string DefaultValue
    {
        get
        {
            if (Modifier is { } modifier)
            {
                return modifier.GetDefaultValue();
            }
            
            return BasicType.GetDefaultValue();
        }
    }
    
    public string SubstitutionModifier
    {
        get
        {
            if (Modifier is { } modifier)
            {
                return modifier.GetSubstitutionModifier();
            }

            return BasicType.GetSubstitutionModifier();
        }
    }
    
    public string GetSubstitutionModifier(object index)
    {
        return string.Format(SubstitutionModifier, index);
    }
}