using JetBrains.Annotations;
using Tomlet.Attributes;

namespace Amethyst.Model;

public class Datapack
{
    /// <summary>
    /// The name of the datapack.
    /// </summary>
    [TomlProperty("name")]
    public required string Name { get; [UsedImplicitly] init; } = Constants.DefaultDatapackName;
    
    /// <summary>
    /// The output directory of the resulting datapack. Already includes the datapack name.
    /// </summary>
    [TomlProperty("output")]
    public required string? Output { get; [UsedImplicitly] init; }

    /// <summary>
    /// The description of the datapack.
    /// </summary>
    [TomlProperty("description")]
    public required string? Description { get; [UsedImplicitly] init; } = Constants.DefaultDatapackDescription;

    /// <summary>
    /// The format version of the datapack.
    /// </summary>
    [TomlProperty("pack_format")]
    public required int? PackFormat { get; [UsedImplicitly] init; } = Constants.DefaultDatapackFormat;
    
    /// <summary>
    /// The absolute path to the output directory where files will be generated into.
    /// </summary>
    [TomlNonSerialized]
    public string OutputDir { get; set; } = null!;

    /// <summary>
    /// List of MCF function paths to be executed every tick.
    /// </summary>
    [TomlNonSerialized]
    public HashSet<string> TickFunctions { get; } = new();
    
    /// <summary>
    /// List of MCF function paths to be executed on load.
    /// </summary>
    [TomlNonSerialized]
    public HashSet<string> LoadFunctions { get; } = new();
    
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public Datapack()
    {
    }
}