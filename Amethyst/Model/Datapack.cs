using Amethyst.Language;
using Amethyst.Utility;
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

    public IEnumerable<string> LoadFunctions => Context.Namespaces.SelectRecursive(ns => ns.Value.Namespaces).SelectMany(ns =>
    {
        return ns.Value.Functions.Where(fn =>
        {
            return fn.Value.attribute_list().Any(Attribute_listContext =>
            {
                return Attribute_listContext.attribute().Any(AttributeContext =>
                {
                    return AttributeContext.GetText() == ATTRIBUTE_LOAD_FUNCTION;
                });
            });
        }).Select(fn =>
        {
            return ns.Value.McFunctionPath + fn.Value.identifier().GetText();
        });
    });
    
    public IEnumerable<string> TickFunctions => Context.Namespaces.SelectRecursive(ns => ns.Value.Namespaces).SelectMany(ns =>
    {
        return ns.Value.Functions.Where(fn =>
        {
            return fn.Value.attribute_list().Any(Attribute_listContext =>
            {
                return Attribute_listContext.attribute().Any(AttributeContext =>
                {
                    return AttributeContext.GetText() == ATTRIBUTE_TICK_FUNCTION;
                });
            });
        }).Select(fn =>
        {
            return ns.Value.McFunctionPath + fn.Value.identifier().GetText();
        });
    });
}