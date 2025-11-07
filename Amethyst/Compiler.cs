using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystParserBaseVisitor<object?>
{
    public Context Context { get; }
    public SourceFile SourceFile { get; internal set; } = null!;
    public Scope Scope { get; internal set; } = null!;
    public Namespace Namespace { get; internal set; } = null!;
    public int TotalRecordCount { get; internal set; }
    public int StackPointer { get; internal set; }
    
    private Dictionary<string, Namespace> _namespaces { get; } = new();

    public Compiler(Context context)
    {
        Context = context;
    }
    
    public void CompileProject()
    {
        foreach (var sourceFile in Context.SourceFiles.Values)
        {
            CompileSourceFile(sourceFile);
        }

        foreach (var ns in _namespaces.Values)
        {
            ns.Dispose();
        }
    }
    
    private void CompileSourceFile(SourceFile sourceFile)
    {
        SourceFile = sourceFile;
        Namespace = GetOrCreateNamespace(sourceFile.Namespace);
        using (Scope = sourceFile.Scope!)
        {
            VisitFile(sourceFile.Ast);
        }
    }

    private Namespace GetOrCreateNamespace(string nsName)
    {
        if (!_namespaces.TryGetValue(nsName, out var ns))
        {
            ns = new Namespace(nsName, Context);
            _namespaces[nsName] = ns;
        }

        return ns;
    }
}