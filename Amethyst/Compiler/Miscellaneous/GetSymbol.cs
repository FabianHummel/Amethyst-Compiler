using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public Symbol GetSymbol(string symbolName, ParserRuleContext context, bool checkExportedSymbols = true)
    {
        if (!TryGetSymbol(symbolName, out var symbol, context, checkExportedSymbols))
        {
            throw new SemanticException($"Symbol '{symbolName}' does not exist in the current scope.", context);
        }
        
        return symbol;
    }
    
    public bool TryGetSymbol(string name, [NotNullWhen(true)] out Symbol? symbol, ParserRuleContext context, bool checkExportedSymbols = true)
    {
        var sourceFile = SourceFile;
        return TryGetSymbolRecursive(name, ref sourceFile, out symbol, context, checkExportedSymbols);
    }

    private bool TryGetSymbolRecursive(string name, ref SourceFile sourceFile, [NotNullWhen(true)] out Symbol? symbol, ParserRuleContext context, bool checkExportedSymbols)
    {
        if (Scope.TryGetSymbol(name, out symbol))
        {
            return true;
        }

        if (checkExportedSymbols && sourceFile.ExportedSymbols.TryGetValue(name, out var declarationContext))
        {
            VisitDeclaration(declarationContext);
            return TryGetSymbolRecursive(name, ref sourceFile, out symbol, context, checkExportedSymbols);
        }

        if (sourceFile.ImportedSymbols.TryGetValue(name, out var resourcePath))
        {
            sourceFile = VisitResource(resourcePath, Constants.DatapackFunctionsDirectory, context);
            // TODO: This does not work because the namespace, registry, etc... is not set to the correct values.
            // if (sourceFile.Scope == null)
            // {
            //     CompileSourceFile(sourceFile);
            // }
            
            using var scope = new DisposableScope(this, sourceFile.Scope);
            var foundSymbol = TryGetSymbolRecursive(name, ref sourceFile, out symbol, context, checkExportedSymbols);
            return foundSymbol;
        }
        
        symbol = null;
        return false;
    }
}