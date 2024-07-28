using System.ComponentModel;
using Amethyst.Model.Types;
using Amethyst.Utility;

namespace Amethyst.Model;

[AttributeUsage(AttributeTargets.Field)]
public class ScaleAttribute : Attribute
{
    public int Scale { get; }
    
    public ScaleAttribute(int scale)
    {
        Scale = scale;
    }
}

public enum BasicType
{
    [Description("int")]
    [Scale(1)]
    Int,
    [Description("dec")]
    [Scale(100)]
    Dec,
    [Description("string")]
    String,
    [Description("bool")]
    [Scale(1)]
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
    
    public int? Scale
    {
        get
        {
            var field = BasicType.GetType().GetField(BasicType.ToString());
            var attribute = field?.GetCustomAttributes(typeof(ScaleAttribute), false).FirstOrDefault() as ScaleAttribute;
            return attribute?.Scale;
        }
    }

    public bool IsScoreboardType => Modifier == null && BasicType is BasicType.Bool or BasicType.Int or BasicType.Dec;
    
    public bool IsStorageType => !IsScoreboardType;
    
    public bool IsBoolean => Modifier == null && BasicType == BasicType.Bool;

    public string? StorageModifier
    {
        get
        {
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
    }
}