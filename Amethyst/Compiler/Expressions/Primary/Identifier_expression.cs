using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public Symbol VisitIdentifier_symbol(string symbolName)
    {
        var sourceFile = SourceFile;

        retry_scope:
        if (Scope.TryGetSymbol(symbolName, out var symbol))
        {
            return symbol;
        }

        retry_source_file:
        if (sourceFile.ExportedSymbols.Remove(symbolName, out var context)) // TODO: I'm reeeeally not sure
        {                                                                   // if the exported symbol should
            VisitDeclaration(context);                                      // be removed entirely...
            goto retry_scope;
        }

        if (sourceFile.ImportedSymbols.TryGetValue(symbolName, out var resourcePath))
        {
            sourceFile = VisitResource(resourcePath, Constants.DATAPACK_FUNCTIONS_DIRECTORY);
            goto retry_source_file;
        }

        throw new SemanticException($"The symbol '{symbolName}' does not exist in the current scope.");
    }
    
    public override AbstractResult VisitIdentifier_expression(AmethystParser.Identifier_expressionContext context)
    {
        var symbolName = context.IDENTIFIER().GetText();
        Symbol? symbol;
        try
        {
            symbol = VisitIdentifier_symbol(symbolName);
        }
        catch (SemanticException e)
        {
            throw new SyntaxException(e.Message, context);
        }
        
        if (symbol is Variable variable)
        {
            if (variable.DataType.Modifier is { } modifier)
            {
                return modifier switch
                {
                    Modifier.Array => new StaticArrayResult
                    {
                        Compiler = this,
                        Context = context,
                        Location = variable.Location,
                        BasicType = variable.DataType.BasicType
                    },
                    Modifier.Object => new StaticObjectResult
                    { 
                        Compiler = this, 
                        Context = context, 
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
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.Dec => new DecimalResult
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location,
                    DecimalPlaces = (variable.DataType as DecimalDataType)!.DecimalPlaces
                },
                BasicType.Bool => new BooleanResult
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.String => new StringResult
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.Array => new DynArrayResult
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.Object => new DynObjectResult
                { 
                    Compiler = this, 
                    Context = context, 
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