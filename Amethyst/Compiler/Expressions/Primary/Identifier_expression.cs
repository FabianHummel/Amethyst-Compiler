using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime.Tree;

namespace Amethyst;

public partial class Compiler
{
    public Symbol VisitIdentifier_path(AmethystParser.IdentifierContext identifierContext)
    {
        string symbolName = null!;
        var ns = Namespace;
        var sourceFile = SourceFile;
        ITerminalNode[] pathsToTraverse = null!;
        if (identifierContext is AmethystParser.Absolute_identifierContext absoluteIdentifierContext)
        {
            symbolName = absoluteIdentifierContext.IDENTIFIER().Last().Symbol.Text;
            var nsName = absoluteIdentifierContext.IDENTIFIER().First().Symbol.Text;
            pathsToTraverse = absoluteIdentifierContext.IDENTIFIER()[1..^1];
            if (!Context.Namespaces.TryGetValue(nsName, out ns))
            {
                throw new SyntaxException($"Namespace '{nsName}' does not exist anywhere in the project.", absoluteIdentifierContext);
            }
        }
        if (identifierContext is AmethystParser.Relative_identifierContext relativeIdentifierContext)
        {
            symbolName = relativeIdentifierContext.IDENTIFIER().Last().Symbol.Text;
            pathsToTraverse = relativeIdentifierContext.IDENTIFIER()[..^1];
        }

        if (!ns.Registries.TryGetValue(Constants.DATAPACK_FUNCTIONS_DIRECTORY, out var sourceFolder))
        {
            throw new SyntaxException("No symbols have been defined in the 'function' registry.", identifierContext);
        }

        for (var i = 0; i < pathsToTraverse.Length - 1; i++)
        {
            var sourceFolderName = pathsToTraverse[i].Symbol.Text;
            if (!sourceFolder.Children.TryGetValue(sourceFolderName, out sourceFolder))
            {
                throw new SyntaxException($"The folder '{sourceFolderName}' does not exist for the specified path.", identifierContext);
            }
        }

        if (pathsToTraverse.Length > 0)
        {
            var sourceFileName = pathsToTraverse[^1].Symbol.Text;
            if (!sourceFolder.SourceFiles.TryGetValue(sourceFileName, out sourceFile))
            {
                throw new SyntaxException("The specified source file does not exist for the specified path.", identifierContext);
            }
        }
        
        try_again:
        if (Scope.TryGetSymbol(symbolName, out var symbol))
        {
            return symbol;
        }

        if (sourceFile.ExportedSymbols.Remove(symbolName, out var declarationContext))
        {
            VisitDeclaration(declarationContext);
            goto try_again;
        }
        
        // TODO: Support for imported symbols

        throw new SyntaxException($"The symbol '{symbolName}' does not exist in the current context.", identifierContext);
    }
    
    public override AbstractResult VisitIdentifier_expression(AmethystParser.Identifier_expressionContext context)
    {
        if (context.identifier() is not { } identifierContext)
        {
            throw new UnreachableException();
        }

        var symbol = VisitIdentifier_path(identifierContext);
        
        if (symbol is Variable variable)
        {
            if (variable.DataType.Modifier is { } modifier)
            {
                return modifier switch
                {
                    Modifier.Array => new StaticArrayResult
                    {
                        Compiler = this,
                        Context = identifierContext,
                        Location = variable.Location,
                        BasicType = variable.DataType.BasicType
                    },
                    Modifier.Object => new StaticObjectResult
                    { 
                        Compiler = this, 
                        Context = identifierContext, 
                        Location = variable.Location, 
                        BasicType = variable.DataType.BasicType
                    },
                    _ => throw new UnreachableException()
                };
            }

            return variable.DataType.BasicType switch
            {
                BasicType.Int => new IntegerResult
                { 
                    Compiler = this, 
                    Context = identifierContext, 
                    Location = variable.Location
                },
                BasicType.Dec => new DecimalResult
                { 
                    Compiler = this, 
                    Context = identifierContext, 
                    Location = variable.Location,
                    DecimalPlaces = (variable.DataType as DecimalDataType)!.DecimalPlaces
                },
                BasicType.Bool => new BooleanResult
                { 
                    Compiler = this, 
                    Context = identifierContext, 
                    Location = variable.Location
                },
                BasicType.String => new StringResult
                { 
                    Compiler = this, 
                    Context = identifierContext, 
                    Location = variable.Location
                },
                BasicType.Array => new DynArrayResult
                { 
                    Compiler = this, 
                    Context = identifierContext, 
                    Location = variable.Location
                },
                BasicType.Object => new DynObjectResult
                { 
                    Compiler = this, 
                    Context = identifierContext, 
                    Location = variable.Location
                },
                _ => throw new UnreachableException()
            };
        }

        if (symbol is Record record)
        {
            throw new NotImplementedException();
        }

        if (symbol is Function function)
        {
            
        }

        throw new UnreachableException();
    }
}