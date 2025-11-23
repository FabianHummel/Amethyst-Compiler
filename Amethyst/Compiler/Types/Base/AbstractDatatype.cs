using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public abstract class AbstractDatatype : IEquatable<AbstractDatatype>
{
    public abstract BasicType BasicType { get; }
    
    public abstract DataLocation DataLocation { get; }
    
    public Modifier? Modifier { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not AbstractDatatype type)
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
    
    public static bool operator ==(AbstractDatatype? left, AbstractDatatype? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }
    
    public static bool operator !=(AbstractDatatype? left, AbstractDatatype? right)
    {
        if (left is null)
        {
            return right is not null;
        }
        return !left.Equals(right);
    }
    
    public bool Equals(AbstractDatatype? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return BasicType == other.BasicType && Modifier == other.Modifier;
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

    public static AbstractDatatype Parse(BasicType basicType, Modifier? modifier)
    {
        return basicType switch
        {
            BasicType.Int => new IntegerDatatype { Modifier = modifier },
            BasicType.Bool => new BooleanDatatype { Modifier = modifier },
            BasicType.String => new StringDatatype { Modifier = modifier },
            BasicType.Array => new ArrayDatatype { Modifier = modifier },
            BasicType.Object => new ObjectDatatype { Modifier = modifier },
            BasicType.Dec => throw new UnreachableException("Decimal type is handled separately."),
            BasicType.Raw => throw new UnreachableException("Raw type is handled separately."),
            BasicType.Entity => new EntityDatatype { Modifier = modifier },
            _ => throw new ArgumentOutOfRangeException(nameof(basicType), $"Unsupported basic type '{basicType}'.")
        };
    }

    public bool IsScoreboardType([NotNullWhen(true)] out AbstractScoreboardDatatype? scoreboardDatatype)
    {
        if (DataLocation == DataLocation.Scoreboard && this is AbstractScoreboardDatatype sbDatatype)
        {
            scoreboardDatatype = sbDatatype;
            return true;
        }
        
        scoreboardDatatype = null;
        return false;
    }
}