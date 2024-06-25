using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystBaseVisitor<object?>
{
    private Context Context { get; }
    private Scope Scope { get; set; } = null!;
    private Namespace Namespace { get; set; } = null!;
    
    public Compiler(Context context)
    {
        Context = context;
    }

    public void Compile()
    {
        foreach (var ns in Context.Namespaces)
        {
            Namespace = ns;
            
            Scope = new Scope
            {
                Name = ns.Functions["_load"].Name,
                Parent = ns.Scope,
                Context = Context
            };
            
            Scope.AddCode("");
            
            foreach (var file in ns.Files)
            {
                VisitFile(file);
            }
        }
    }
}