using JetBrains.Annotations;
using Tomlet.Attributes;

namespace Amethyst.Model;

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