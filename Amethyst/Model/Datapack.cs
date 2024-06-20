namespace Amethyst.Model;

public class Datapack
{
    /// <summary>
    /// The name of the datapack.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// The output directory of the resulting datapack. Already includes the datapack name.
    /// </summary>
    public string OutputDir { get; set; } = null!;
    
    public List<Stmt.Function> LoadFunctions { get; } = new();
    public List<Stmt.Function> TickFunctions { get; } = new();
}