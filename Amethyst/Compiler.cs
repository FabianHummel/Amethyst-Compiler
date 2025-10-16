using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystParserBaseVisitor<object?>
{
    internal Context Context { get; }
    internal int TotalRecordCount { get; set; }
    internal int StackPointer { get; set; }
    
    internal SourceFile SourceFile { get; set; } = null!;
    internal Scope Scope { get; set; } = null!;
    
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
    }
    
    private void CompileSourceFile(SourceFile sourceFile)
    {
        SourceFile = sourceFile;
        Scope = sourceFile.Scope!;
        
        foreach (var entryPoint in sourceFile.EntryPointFunctions.Values)
        {
            VisitFunctionDeclaration(entryPoint);
        }
            
        foreach (var (symbolName, symbol) in sourceFile.ExportedSymbols)
        {
            if (Scope.Symbols.ContainsKey(symbolName))
            {
                continue;
            }
                    
            VisitDeclaration(symbol);
        }
    }
}