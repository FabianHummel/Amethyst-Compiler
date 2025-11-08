namespace Amethyst;

/// <summary>Provides indexed access to elements of a type.</summary>
public interface IIndexable
{
    /// <summary>Get the value at the specified index. The index may be any value, although bound checks
    /// are only supported with constant values.</summary>
    /// <param name="index">The specified index in the element list.</param>
    /// <returns>The value at the specified <paramref name="index" />.</returns>
    AbstractValue GetIndex(AbstractValue index);
}