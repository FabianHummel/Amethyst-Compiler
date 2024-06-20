using Amethyst.Model;

namespace Amethyst.Compiler;

public class Compiler
{
    private IList<Stmt> Statements { get; }
    private Context Context { get; }
    
    public Compiler(IList<Stmt> stmts, Context context)
    {
        Statements = stmts;
        Context = context;
    }
    
    public void Compile()
    {
        
    }
}