using System.Diagnostics;

namespace Amethyst;

public abstract partial class AbstractNumericValue : AbstractAmethystValue
{
    /// <summary>
    /// Ensures that this number is stored in storage, likely to be able to use it as macro input.
    /// </summary>
    /// <returns>A runtime value pointing to a storage location</returns>
    public IRuntimeValue EnsureInStorage()
    {
        if (this is not IRuntimeValue runtimeValue)
        {
            throw new UnreachableException("Cannot call this on a constant value");
        }
        
        var location = runtimeValue.NextFreeLocation();
        
        AddCode($"execute store result storage amethyst: {location} {DataType.StorageModifier} run scoreboard players get {runtimeValue.Location} amethyst");

        var clone = (IRuntimeValue)MemberwiseClone();
        clone.Location = location;
        clone.IsTemporary = true;
        return clone;
    }
}