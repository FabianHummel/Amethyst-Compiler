using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Antlr4.Runtime;
using static Amethyst.Model.Constants;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Gets a symbol by name in the current scope or any of its ancestor scopes. This function
    /// calls the non-throwing method <see cref="TryGetSymbol" /> and throws an exception if the symbol was
    /// not found.</summary>
    /// <param name="symbolName">The symbol's name to search for.</param>
    /// <param name="context">The parser rule context used for error handling.</param>
    /// <param name="checkExportedSymbols">Whether to check exported symbols of the current source files.</param>
    /// <returns>The symbol if it was found, otherwise throws an Exception.</returns>
    /// <exception cref="SemanticException">The symbol was not found in the current scope or any of its
    /// ancestor scopes.</exception>
    /// <seealso cref="TryGetSymbol" />
    public Symbol GetSymbol(string symbolName, ParserRuleContext context, bool checkExportedSymbols = true)
    {
        if (!TryGetSymbol(symbolName, out var symbol, context, checkExportedSymbols))
        {
            throw new SemanticException($"Symbol '{symbolName}' does not exist in the current scope.", context);
        }
        
        return symbol;
    }

    /// <summary>Tries to find a symbol by name. Unlike <see cref="GetSymbol" />, this method does not
    /// raise an exception when the symbol was not found, but rather returns false with no result.</summary>
    /// <param name="name">The symbol's name to search for.</param>
    /// <param name="symbol">The resulting symbol if it was found, otherwise null.</param>
    /// <param name="context">The parser rule context used for error handling.</param>
    /// <param name="checkExportedSymbols">Whether to check exported symbols of the current source files.</param>
    /// <returns>True, if the symbol was found, otherwise false.</returns>
    /// <seealso cref="GetSymbol" />
    public bool TryGetSymbol(string name, [NotNullWhen(true)] out Symbol? symbol, ParserRuleContext context, bool checkExportedSymbols = true)
    {
        return TryGetSymbolRecursive(name, out symbol, context, checkExportedSymbols);
    }

    /// <inheritdoc cref="TryGetSymbol" />
    /// <summary>Internal method for recursively traversing a scope's ancestors in order to find a symbol
    /// by its name.</summary>
    private bool TryGetSymbolRecursive(string name, [NotNullWhen(true)] out Symbol? symbol, ParserRuleContext context, bool checkExportedSymbols)
    {
        if (Scope.TryGetSymbol(name, out symbol))
        {
            return true;
        }

        if (checkExportedSymbols && SourceFile.ExportedSymbols.TryGetValue(name, out var declarationContext))
        {
            if (SourceFile.DeclarationCache.TryGetValue(declarationContext, out symbol))
            {
                return true;
            }

            var scopeBackup = new Scope.GlobalBackup(this, SourceFile.Scope!);
            symbol = VisitDeclaration(declarationContext);
            SourceFile.DeclarationCache.Add(declarationContext, symbol);
            scopeBackup.Dispose(disposeScope: false);
            return true;
        }

        if (SourceFile.ImportedSymbols.TryGetValue(name, out var resource))
        {
            var sourceFile = GetSourceFile(resource, DatapackFunctionsDirectory, context);
            using var scope = new SourceFile.GlobalBackup(this, sourceFile);
            var foundSymbol = TryGetSymbolRecursive(name, out symbol, context, checkExportedSymbols);
            return foundSymbol;
        }
        
        symbol = null;
        return false;
    }

    /// <summary>Helper method for ensuring that a symbol is not defined in the current scope or any of its
    /// ancestor scopes. However, if the calling scope is the root scope, as in representing the top-level,
    /// the method always returns true, indicating that the symbol must not be created a second time.</summary>
    /// <param name="name">The symbol's name to search for.</param>
    /// <param name="symbol">The resulting symbol if it was found, otherwise null.</param>
    /// <param name="context">The parser rule context used for error handling.</param>
    /// <returns>True, if the symbol was found, otherwise false.</returns>
    /// <exception cref="SymbolAlreadyDeclaredException">The symbol is already declared in the current or
    /// any ancestor's scope while not in the top-level.</exception>
    private bool EnsureSymbolIsNewOrGetRootSymbol(string name, ParserRuleContext context, [NotNullWhen(true)] out Symbol? symbol)
    {
        if (!TryGetSymbol(name, out symbol, context, checkExportedSymbols: false))
        {
            return false;
        }

        if (Scope.IsRoot)
        {
            return true;
        }
            
        throw new SymbolAlreadyDeclaredException(name, context);
    }
}