using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst.Utility;

public static class PipelineExtensions
{
    public static AmethystParser.FileContext Parse(this string source, Namespace context)
    {
        return new Parser(source, context).Parse();
    }
    
    public static void Compile(this IEnumerable<AmethystParser.FileContext> files, Context context)
    {
        new Compiler(files, context).Compile();
    }
}