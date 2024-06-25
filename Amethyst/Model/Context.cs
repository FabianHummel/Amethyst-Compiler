namespace Amethyst.Model;

public class Context
{
    public string? MinecraftRoot { get; set; }
    public string SourcePath { get; set; } = null!;
    public Datapack? Datapack { get; set; }
    public Resourcepack? Resourcepack { get; set; }
    public List<Namespace> Namespaces { get; } = new();
}