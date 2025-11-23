using static Amethyst.Model.Constants;

namespace Amethyst.Model;

/// <summary>The runtime location of an Amethyst variable. It contains the enclosing namespace, the
/// location's name and the data location where to look for the value. In Minecraft, a value can either
/// be stored in a scoreboard for simple, numeric types or in storage for complex types such as arrays
/// or objects.</summary>
public class Location
{
    /// <summary>The enclosing namespace of where the variable was initially declared.</summary>
    public string Namespace { get; init; }

    /// <summary>The internal name of the variable, which is usually just the current stack pointer
    /// location.</summary>
    public required string Name { get; init; }

    /// <inheritdoc cref="Model.DataLocation" />
    public required DataLocation DataLocation { get; init; }

    /// <summary>Creates a new location value-object with an optional namespace name. If it is not
    /// supplied, it defaults to the <see cref="Constants.InternalNamespaceName" />.</summary>
    /// <param name="ns"></param>
    public Location(string? ns = null)
    {
        Namespace = ns ?? InternalNamespaceName;
    }

    /// <summary>Converts this object to the actual path of a variable's location in MCFunction language.</summary>
    /// <example>
    ///     <list type="bullet"><item>Namespace = <c>"my_namespace"</c></item>
    ///         <item>Name = <c>"var_name"</c></item></list>
    ///     <p><see cref="Model.DataLocation.Storage" /> → <i>/data get storage</i>
    ///     <c>my_namespace: var_name</c></p>
    ///     <p><see cref="Model.DataLocation.Scoreboard" /> → <i>/scoreboard players get</i>
    ///     <c>var_name my_namespace</c></p>
    /// </example>
    /// <returns>The actual path where to access the variable bound to this location.</returns>
    public override string ToString()
    {
        if (DataLocation == DataLocation.Scoreboard)
        {
            return $"{Name} {Namespace}";
        }

        return $"{Namespace}: {Name}";
    }

    /// <summary>Converts this location to a text component representation for use in Minecraft commands.</summary>
    /// <example>
    ///     <list type="bullet"><item>Namespace = <c>"my_namespace"</c></item>
    ///         <item>Name = <c>"var_name"</c></item></list>
    ///     <p><see cref="Model.DataLocation.Storage" /> → <c>{storage:'my_namespace:',nbt:'var_name'}</c>
    ///     </p>
    ///     <p><see cref="Model.DataLocation.Scoreboard" /> →
    ///     <c>{score:{objective:'my_namespace',name:'var_name'}}</c></p>
    /// </example>
    /// <returns>The text component string.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string ToTextComponent()
    {
        if (DataLocation == DataLocation.Scoreboard)
        {
            return $"{{score:{{objective:'{Namespace}',name:'{Name}'}}}}";
        }
        
        if (DataLocation == DataLocation.Storage)
        {
            return $"{{storage:'{Namespace}:',nbt:'{Name}'}}";
        }
        
        throw new InvalidOperationException($"Unknown data location '{DataLocation}'.");
    }

    /// <summary>Implicitly converts this location to its string representation by calling
    /// <see cref="ToString" />.</summary>
    /// <param name="location">The location to convert</param>
    /// <returns>The string representation ready to be used in the datapack code.</returns>
    public static implicit operator string(Location location) => location.ToString();

    /// <summary>Creates a new instance of <see cref="Location" /> in the storage at the specified
    /// <paramref name="location" /> and optionally the namespace.</summary>
    /// <param name="location">The location within storage.</param>
    /// <param name="ns">The storage's namespace.</param>
    /// <returns>The newly created <see cref="Location" /> instance.</returns>
    public static Location Storage(string location, string? ns = null)
    {
        return new Location(ns)
        {
            DataLocation = DataLocation.Storage,
            Name = location
        };
    }
    
    /// <inheritdoc cref="Storage(string,string?)" />
    public static Location Storage(int location, string? ns = null)
    {
        return Storage(location.ToString(), ns);
    }

    /// <summary>Creates a new instance of <see cref="Location" /> in the scoreboard at the specified
    /// <paramref name="location" /> and optionally the namespace.</summary>
    /// <param name="location">The scoreboard player name.</param>
    /// <param name="ns">The scoreboard objective name.</param>
    /// <returns>The newly created <see cref="Location" /> instance.</returns>
    public static Location Scoreboard(string location, string? ns = null)
    {
        return new Location(ns)
        {
            DataLocation = DataLocation.Scoreboard,
            Name = location
        };
    }
    
    /// <inheritdoc cref="Scoreboard(string,string?)" />
    public static Location Scoreboard(int location, string? ns = null)
    {
        return Scoreboard(location.ToString(), ns);
    }

    /// <summary>Returns this value's representation as a macro placeholder.</summary>
    /// <returns>The macro placeholder.</returns>
    public string ToMacroPlaceholder()
    {
        return $"$({Name})";
    }
}