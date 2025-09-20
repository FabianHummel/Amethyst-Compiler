namespace Amethyst.Model;

public class Datapack
{
    public required Context Context { get; init; }
    
    /// <summary>
    /// The name of the datapack.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// The output directory of the resulting datapack. Already includes the datapack name.
    /// </summary>
    public string OutputDir { get; set; } = null!;

    /// <summary>
    /// List of MCF function paths to be executed every tick.
    /// </summary>
    public HashSet<string> TickFunctions { get; set; } = new();
    
    /// <summary>
    /// List of MCF function paths to be executed on load.
    /// </summary>
    public HashSet<string> LoadFunctions { get; set; } = new();
}