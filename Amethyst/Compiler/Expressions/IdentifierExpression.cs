using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitIdentifierExpression(AmethystParser.IdentifierExpressionContext context)
    {
        var symbolName = context.IDENTIFIER().GetText();
        var symbol = GetSymbol(symbolName, context);
        
        if (symbol is Variable variable)
        {
            if (variable.Datatype.Modifier is { } modifier)
            {
                return modifier switch
                {
                    Modifier.Array => new RuntimeStaticArray
                    {
                        Compiler = this,
                        Context = context,
                        Location = variable.Location,
                        BasicType = variable.Datatype.BasicType
                    },
                    Modifier.Object => new RuntimeStaticObject
                    { 
                        Compiler = this, 
                        Context = context, 
                        Location = variable.Location, 
                        BasicType = variable.Datatype.BasicType
                    },
                    _ => throw new InvalidOperationException($"Invalid type modifier '{modifier}'.")
                };
            }

            return variable.Datatype.BasicType switch
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
                    DecimalPlaces = (variable.Datatype as DecimalDatatype)!.DecimalPlaces
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
                BasicType.Raw => new RawLocation
                {
                    Compiler = this,
                    Context = context,
                    Location = variable.Location
                },
                BasicType.Entity => new RuntimeEntity
                {
                    Compiler = this,
                    Context = context,
                    Location = variable.Location
                },
                _ => throw new InvalidOperationException($"Invalid basic type '{variable.Datatype.BasicType}'.")
            };
        }

        if (symbol is Record record)
        {
            throw new NotImplementedException("TODO: Create record from datatype");
        }

        if (symbol is Function function)
        {
            // HINT: Traditionally, functions are values just like any other value, but not in amethyst. I'm not dealing with that shit
            throw new SemanticException("Functions cannot be used as values.", context);
        }

        throw new InvalidOperationException($"Invalid symbol '{symbol.GetType()}'.");
    }
}