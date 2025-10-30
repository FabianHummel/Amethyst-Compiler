using JetBrains.Annotations;
using Tomlet.Attributes;

namespace Amethyst.Model;

/// <summary>The project's configuration that is parsed from the <c>amethyst.toml</c> file in the root
/// directory of the project. It includes data such as the project's ID, the path to Minecraft's root
/// directory and datapack / resourcepack configurations.</summary>
/// <seealso cref="Processor.ParseConfiguration" />
public class Configuration
{
    [TomlProperty("id")]
    public required string ProjectId { get; [UsedImplicitly] init; }
    
    [TomlProperty("minecraft")]
    public required string? MinecraftRoot { get; [UsedImplicitly] init; }
    
    [TomlProperty("datapack")]
    public required Datapack? Datapack { get; [UsedImplicitly] init; }
    
    [TomlProperty("resourcepack")]
    public required Resourcepack? Resourcepack { get; [UsedImplicitly] init; }
    
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public Configuration()
    {
    }
}