using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public Symbol VisitIdentifierSymbol(string symbolName)
    {
        var sourceFile = SourceFile;

        return SearchSymbol();

        Symbol SearchSymbol()
        {
            if (Scope.TryGetSymbol(symbolName, out var symbol))
            {
                return symbol;
            }

            if (sourceFile.ExportedSymbols.TryGetValue(symbolName, out var context))
            {
                VisitDeclaration(context);
                return SearchSymbol();
            }

            if (sourceFile.ImportedSymbols.TryGetValue(symbolName, out var resourcePath))
            {
                sourceFile = VisitResource(resourcePath, Constants.DATAPACK_FUNCTIONS_DIRECTORY);
                var previousScope = Scope;
                Scope = sourceFile.RootScope;
                SearchSymbol();
                Scope = previousScope;
            }
            
            throw new SemanticException($"The symbol '{symbolName}' does not exist in the current scope.");
        }
    }
    
    public override AbstractValue VisitIdentifierExpression(AmethystParser.IdentifierExpressionContext context)
    {
        var symbolName = context.IDENTIFIER().GetText();
        Symbol? symbol;
        try
        {
            symbol = VisitIdentifierSymbol(symbolName);
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
                    Modifier.Array => new RuntimeStaticArray
                    {
                        Compiler = this,
                        Context = context,
                        Location = variable.Location,
                        BasicType = variable.DataType.BasicType
                    },
                    Modifier.Object => new RuntimeStaticObject
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
                BasicType.Int => new RuntimeInteger
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.Dec => new RuntimeDecimal
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location,
                    DecimalPlaces = (variable.DataType as DecimalDataType)!.DecimalPlaces
                },
                BasicType.Bool => new RuntimeBoolean
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.String => new RuntimeString
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.Array => new RuntimeDynamicArray
                { 
                    Compiler = this, 
                    Context = context, 
                    Location = variable.Location
                },
                BasicType.Object => new RuntimeDynamicObject
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