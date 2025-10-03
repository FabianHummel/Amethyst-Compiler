using JetBrains.Annotations;
using Tomlet.Attributes;

namespace Amethyst.Model;

public class Configuration
{
    [TomlProperty("id")]
    public required string ProjectId { get; init; }
    
    [TomlProperty("minecraft")]
    public required string? MinecraftRoot { get; set; }
    
    [TomlProperty("datapack")]
    public required Datapack? Datapack { get; init; }
    
    [TomlProperty("resourcepack")]
    public required Resourcepack? Resourcepack { get; init; }
    
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public Configuration()
    {
    }
}