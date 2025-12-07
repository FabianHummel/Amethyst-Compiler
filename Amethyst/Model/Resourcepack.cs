using JetBrains.Annotations;
using Tomlet.Attributes;

namespace Amethyst.Model;

/// <summary>The resourcepack configuration. This is part of the main <see cref="Configuration" /> and
/// contains settings related to the output resourcepack, such as its name, description, and format
/// version.</summary>
/// <seealso cref="Datapack" />
public class Resourcepack
{
    /// <summary>The name of the resourcepack.</summary>
    [TomlProperty("name")]
    public required string Name { get; [UsedImplicitly] init; } = Constants.DefaultResourcepackName;

    /// <summary>The output directory of the resulting resourcepack. Already includes the resourcepack
    /// name.</summary>
    [TomlProperty("output")]
    public required string? Output { get; [UsedImplicitly] init; }

    /// <summary>The description of the resourcepack.</summary>
    [TomlProperty("description")]
    public required string? Description { get; [UsedImplicitly] init; } = Constants.DefaultResourcepackDescription;

    /// <summary>The format version of the resourcepack.</summary>
    [TomlProperty("pack_format")]
    public required int? PackFormat { get; [UsedImplicitly] init; } = Constants.DefaultResourcepackFormat;

    [TomlProperty("icon")]
    public required string? IconPath { get; [UsedImplicitly] init; }
    
    /// <summary>The absolute path of the output directory where files will be written to.</summary>
    [TomlNonSerialized]
    public string OutputDir { get; set; } = null!;
    
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public Resourcepack()
    {
    }
}