using Amethyst.Utility;

namespace Amethyst.Model;

/// <summary>A representation of a datatype in the preprocessor. Currently only contains a basic type,
/// but will include more complex types in the future, like already implemented in the main compiler
/// with <see cref="Modifier" />. Includes functionality to compare different datatypes for equality.</summary>
public class PreprocessorDatatype
{
    public required BasicPreprocessorType BasicType { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not PreprocessorDatatype type)
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
    
    public static bool operator ==(PreprocessorDatatype? left, PreprocessorDatatype? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }
    
    public static bool operator !=(PreprocessorDatatype? left, PreprocessorDatatype? right)
    {
        if (left is null)
        {
            return right is not null;
        }
        return !left.Equals(right);
    }
}