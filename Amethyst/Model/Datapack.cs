using static Amethyst.Constants;

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

    public IEnumerable<string> LoadFunctions => Context.Namespaces.SelectMany(ns =>
    {
        return ns.Functions.Where(fn =>
        {
            return fn.Value.Attributes.Contains(ATTRIBUTE_LOAD_FUNCTION);
        }).Select(fn =>
        {
            return fn.Value.McFunctionPath;
        });
    });
    
    public IEnumerable<string> TickFunctions => Context.Namespaces.SelectMany(ns =>
    {
        return ns.Functions.Where(fn =>
        {
            return fn.Value.Attributes.Contains(ATTRIBUTE_TICK_FUNCTION);
        }).Select(fn =>
        {
            return fn.Value.McFunctionPath;
        });
    });
}