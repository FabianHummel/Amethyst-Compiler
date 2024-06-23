namespace Amethyst.Model;

public class Resourcepack
{
    public required Context Context { get; init; }
    
    /// <summary>
    /// The name of the resourcepack.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// The output directory of the resulting resourcepack. Already includes the resourcepack name.
    /// </summary>
    public string OutputDir { get; set; } = null!;
}