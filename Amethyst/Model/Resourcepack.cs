using JetBrains.Annotations;
using Tomlet.Attributes;

namespace Amethyst.Model;

public class Resourcepack
{
    /// <summary>
    /// The name of the resourcepack.
    /// </summary>
    [TomlProperty("name")]
    public required string Name { get; init; } = Constants.DEFAULT_RESOURCEPACK_NAME;
    
    /// <summary>
    /// The output directory of the resulting resourcepack. Already includes the resourcepack name.
    /// </summary>
    [TomlProperty("output")]
    public required string? Output { get; init; }

    /// <summary>
    /// The description of the resourcepack.
    /// </summary>
    [TomlProperty("description")]
    public required string? Description { get; init; } = Constants.DEFAULT_RESOURCEPACK_DESCRIPTION;

    /// <summary>
    /// The format version of the resourcepack.
    /// </summary>
    [TomlProperty("pack_format")]
    public required int? PackFormat { get; init; } = Constants.DEFAULT_RESOURCEPACK_FORMAT;

    /// <summary>
    /// The absolute path of the output directory where files will be written to.
    /// </summary>
    [TomlNonSerialized]
    public string OutputDir { get; set; } = null!;
    
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public Resourcepack()
    {
    }
}