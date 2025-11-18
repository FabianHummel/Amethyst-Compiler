namespace Amethyst.Model;

/// <summary>Represents a record definition within the Amethyst source code. Contextually very similar
/// to a <see cref="Record" />.</summary>
public class Variable : Symbol
{
    /// <summary>The variable's name.</summary>
    public required string Name { get; init; }
    
    /// <summary>The variable's location.</summary>
    public required Location Location { get; init; }
    
    /// <summary>The variable's datatype</summary>
    public required AbstractDatatype Datatype { get; init; }

    /// <summary>A set of attributes linked to the variable. For example, a variable with an
    /// <see cref="Constants.AttributeNoMangle" /> attribute will preserve its original name given in the
    /// source code.</summary>
    public required HashSet<string> Attributes { get; init; }

    /// <summary>Creates a representation of this variable looking similar to the source code declaration.
    /// Mostly for debugging purposes.</summary>
    /// <returns>A pretty printed version of this variable.</returns>
    public override string ToString()
    {
        var attributes = Attributes.Count > 0 ? $"[{string.Join(", ", Attributes)}] " : "";
        return $"{attributes}{Datatype} ({Location})";
    }
}