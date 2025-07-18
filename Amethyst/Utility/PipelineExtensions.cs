using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst.Utility;

public static class PipelineExtensions
{
    public static AmethystParser.FileContext Parse(this Context context, Stream stream, string fileName, Namespace? ns, out Namespace @namespace)
    {
        return new Parser(context, stream, fileName, ns).Parse(out @namespace);
    }
    
    public static void Compile(this Context context)
    {
        new Compiler(context).Compile();
    }
}