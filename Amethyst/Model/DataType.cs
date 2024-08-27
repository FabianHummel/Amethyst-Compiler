using Amethyst.Attributes;
using Amethyst.Utility;

namespace Amethyst.Model;

public enum BasicType
{
    [Description("int")]
    [DefaultValue(0)]
    [Scale(1)]
    Int,
    [Description("dec")]
    [DefaultValue(0.0)]
    [Scale(100)]
    Dec,
    [Description("string")]
    [DefaultValue("")]
    String,
    [Description("bool")]
    [DefaultValue(false)]
    [Scale(1)]
    Bool,
    [Description("array")]
    [DefaultValue("{_:0}")]
    [SubstitutionModifier("[{0}]._")]
    Array,
    [Description("object")]
    Object,
}

public enum Modifier
{
    [Description("[]")]
    [DefaultValue("[]")]
    [SubstitutionModifier("[{0}]")]
    Array,
    [Description("{}")]
    [DefaultValue("{}")]
    [SubstitutionModifier(".{0}")]
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
    
    public int? Scale => BasicType.GetScale();

    public bool IsScoreboardType => Modifier == null && BasicType is BasicType.Bool or BasicType.Int or BasicType.Dec;
    
    public bool IsStorageType => !IsScoreboardType;
    
    public bool IsBoolean => Modifier == null && BasicType == BasicType.Bool;

    public string? StorageModifier
    {
        get
        {
            if (!IsScoreboardType)
            {
                return null;
            }
        
            if (BasicType == BasicType.Bool)
            {
                return $"byte {1 / Scale}";
            }
            if (BasicType == BasicType.Int)
            {
                return $"int {1 / Scale}";
            }
            if (BasicType == BasicType.Dec)
            {
                return $"double {1 / Scale}";
            }
        
            return null;
        }
    }

    public object DefaultValue
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
            
            return string.Empty;
        }
    }
    
    public string GetSubstitutionModifier(object index)
    {
        return string.Format(SubstitutionModifier, index);
    }
}