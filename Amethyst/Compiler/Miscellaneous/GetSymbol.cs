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
        return TryGetSymbolRecursive(name, out symbol, context, checkExportedSymbols);
    }

    private bool TryGetSymbolRecursive(string name, [NotNullWhen(true)] out Symbol? symbol, ParserRuleContext context, bool checkExportedSymbols)
    {
        if (Scope.TryGetSymbol(name, out symbol))
        {
            return true;
        }

        if (checkExportedSymbols && SourceFile.ExportedSymbols.TryGetValue(name, out var declarationContext))
        {
            VisitDeclaration(declarationContext);
            return TryGetSymbolRecursive(name, out symbol, context, checkExportedSymbols);
        }

        if (SourceFile.ImportedSymbols.TryGetValue(name, out var resourcePath))
        {
            var sourceFile = GetSourceFile(resourcePath, Constants.DatapackFunctionsDirectory, context);
            using var scope = new SourceFile.GlobalBackup(this, sourceFile);
            var foundSymbol = TryGetSymbolRecursive(name, out symbol, context, checkExportedSymbols);
            return foundSymbol;
        }
        
        symbol = null;
        return false;
    }
}