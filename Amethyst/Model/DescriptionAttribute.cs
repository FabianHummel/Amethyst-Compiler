namespace Amethyst.Model;

/// <summary>Attribute to provide a description for enum fields. Used in a wide variety of places for
/// code generation.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class DescriptionAttribute : Attribute
{
    /// <summary>The description associated with the enum field.</summary>
    public string Description { get; }

    /// <summary>Constructor for the DescriptionAttribute.</summary>
    /// <param name="description">The description to associate with the enum field.</param>
    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}