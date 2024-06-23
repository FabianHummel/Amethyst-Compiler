using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystBaseVisitor<object?>
{
    private IEnumerable<AmethystParser.FileContext> Files { get; }
    private Context Context { get; }
    
    public Compiler(IEnumerable<AmethystParser.FileContext> files, Context context)
    {
        Files = files;
        Context = context;
    }

    public void Compile()
    {
        foreach (var file in Files)
        {
            VisitFile(file);
        }
    }
}