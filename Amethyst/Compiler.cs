using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;
using static Amethyst.Constants;

namespace Amethyst;

/// <summary>The compiler is a collection of many partial classes that traverse the source code and
/// generate mcfunction code. It extends from <see cref="AmethystParseListener" /> that is
/// automatically generated through ANTLR4 based on a grammar file.</summary>
public partial class Compiler : AmethystParserBaseVisitor<object?>
{
    /// <summary>The compilation context that is created in the <see cref="Processor" /> holds information
    /// and configurations of the project that is compiled.</summary>
    public Context Context { get; }

    /// <summary>The source file that is currently being compiled. It is set automatically upon entering
    /// <see cref="CompileSourceFile" />.</summary>
    public SourceFile SourceFile { get; internal set; } = null!;

    /// <summary>The current scope during compilation. The root scope is automatically set upon entering
    /// <see cref="CompileSourceFile" />.</summary>
    public Scope Scope { get; internal set; } = null!;

    /// <summary>The current namespace during compilation. It is set automatically upon entering
    /// <see cref="CompileSourceFile" />.</summary>
    public Namespace Namespace { get; internal set; } = null!;

    /// <summary>The current stack pointer during compilation. This is basically an incrementing location
    /// where new variabled will be allocated at.</summary>
    public int StackPointer { get; internal set; }

    /// <summary>A list of all namespaces that are used in compilation.</summary>
    private Dictionary<string, Namespace> _namespaces { get; } = new();

    /// <summary>Creates an instance of the compiler using the given compilation
    /// <paramref name="context" />.</summary>
    /// <param name="context">Sets the <see cref="Context" /></param>
    public Compiler(Context context)
    {
        Context = context;
    }

    /// <summary>Compiles the entire project by iterating over all source files and compiling them
    /// individually with <see cref="CompileSourceFile" />. After compilation, all namespaces are disposed,
    /// ultimately generating initialization code that needs to run before anything else.</summary>
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

    /// <summary>Compiles a single source file. This sets the following properties:
    /// <see cref="SourceFile" />, <see cref="Scope" /> and <see cref="Namespace" />, whereas the namespace
    /// needs to be created if it does not yet exist. A source file is compiled by iterating over all entry
    /// points, which are functions with an <see cref="Constants.AttributeLoadFunction" /> or
    /// <see cref="Constants.AttributeTickFunction" /> attribute. The rest is simply traversing the
    /// abstract syntax tree of the function body.</summary>
    /// <param name="sourceFile">The source file that is compiled.</param>
    private void CompileSourceFile(SourceFile sourceFile)
    {
        SourceFile = sourceFile;
        Scope = sourceFile.Scope!;
        Namespace = GetOrCreateNamespace(sourceFile.Namespace);
        
        foreach (var entryPoint in sourceFile.EntryPointFunctions.Values)
        {
            VisitFunctionDeclaration(entryPoint);
        }
        
        // TODO: change this code, because the exported symbols don't automatically belong to the dependency graph.
        //  If a symbol should be compiled nevertheless, the user must add a [not_unused] or similar attribute. 
        foreach (var (symbolName, symbol) in sourceFile.ExportedSymbols)
        {
            if (Scope.Symbols.ContainsKey(symbolName))
            {
                continue;
            }
            
            VisitDeclaration(symbol);
        }
    }

    /// <summary>Gets or creates a namespace that holds information regarding compilation that is tied to a
    /// namespace like initialization functions.</summary>
    /// <param name="nsName">The namespace's name</param>
    /// <returns>An already existing namespace or a newly created one if there wasn't one before.</returns>
    private Namespace GetOrCreateNamespace(string nsName)
    {
        if (!_namespaces.TryGetValue(nsName, out var ns))
        {
            ns = new Namespace(nsName, Context);
            _namespaces[nsName] = ns;
        }

        return ns;
    }

    /// <summary>Gets a source file based on the given resource path and registry name.</summary>
    /// <param name="resourcePath">The resource path that may optionally contain a namespace prefix. This
    /// is equivalent to actual namespaces that one would find in Minecraft.</param>
    /// <param name="registryName">The registry name such as "function", "advancement" or "predicate".</param>
    /// <param name="context">The parser context that is used for error reporting.</param>
    /// <returns>The source file at the specified path.</returns>
    /// <exception cref="SyntaxException">The resource path does not conform to the expected format of
    /// <c>&lt;namespace&gt;?:&lt;path/to/resource&gt;</c>.</exception>
    /// <exception cref="SemanticException">The source file does not exist at the specified path.</exception>
    private SourceFile GetSourceFile(string resourcePath, string registryName, ParserRuleContext context)
    {
        var nsName = SourceFile.Namespace;

        if (resourcePath.Split(':', 2) is { Length: > 1 } parts)
        {
            nsName = parts[0];
            resourcePath = parts[1];
            if (resourcePath == null)
            {
                throw new SyntaxException("Resource path cannot be empty.", context);
            }
        }

        var sourceFilePath = Path.Combine(DatapackRootDir, nsName, registryName, resourcePath);
        
        if (!Context.SourceFiles.TryGetValue(sourceFilePath, out var sourceFile))
        {
            throw new SemanticException($"The source file '{sourceFilePath}' does not exist for the specified path.", context);
        }

        return sourceFile;
    }
}