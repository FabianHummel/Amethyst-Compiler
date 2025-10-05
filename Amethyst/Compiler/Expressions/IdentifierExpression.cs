using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    private Symbol GetSymbol(string symbolName, ParserRuleContext context)
    {
        var sourceFile = SourceFile;
        return FindSymbolRecursive();

        Symbol FindSymbolRecursive()
        {
            if (Scope.TryGetSymbol(symbolName, out var symbol))
            {
                return symbol;
            }

            if (sourceFile.ExportedSymbols.TryGetValue(symbolName, out var declarationContext))
            {
                VisitDeclaration(declarationContext);
                return FindSymbolRecursive();
            }

            if (sourceFile.ImportedSymbols.TryGetValue(symbolName, out var resourcePath))
            {
                sourceFile = VisitResource(resourcePath, Constants.DatapackFunctionsDirectory, context);
                var previousScope = Scope;
                Scope = sourceFile.RootScope;
                symbol = FindSymbolRecursive();
                Scope = previousScope;
                return symbol;
            }

            throw new SemanticException($"Symbol '{symbolName}' does not exist in the current scope.", context);
        }
    }
    
    public override AbstractValue VisitIdentifierExpression(AmethystParser.IdentifierExpressionContext context)
    {
        var symbolName = context.IDENTIFIER().GetText();
        var symbol = GetSymbol(symbolName, context);
        
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
                    _ => throw new InvalidOperationException($"Invalid type modifier '{modifier}'.")
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
                _ => throw new InvalidOperationException($"Invalid basic type '{variable.DataType.BasicType}'.")
            };
        }

        if (symbol is Record record)
        {
            throw new NotImplementedException("TODO: Create record from datatype");
        }

        if (symbol is Function function)
        {
            throw new NotImplementedException("TODO: What to do here?");
        }

        throw new InvalidOperationException($"Invalid symbol '{symbol.GetType()}'.");
    }
}