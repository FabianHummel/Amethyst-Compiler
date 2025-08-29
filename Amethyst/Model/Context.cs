namespace Amethyst.Model;

public class Context
{
    public string ProjectId { get; set; } = null!;
    public CompilerFlags CompilerFlags { get; set; }
    public string? MinecraftRoot { get; set; }
    public string SourcePath { get; set; } = null!;
    public Datapack? Datapack { get; set; }
    public Resourcepack? Resourcepack { get; set; }
    public Dictionary<string, Namespace> Namespaces { get; } = new();

    public void Clear()
    {
        Namespaces.Clear();
    }
}