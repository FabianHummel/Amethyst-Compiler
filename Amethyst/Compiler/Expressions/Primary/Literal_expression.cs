using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitLiteral_expression(AmethystParser.Literal_expressionContext context)
    {
        if (context.literal() is not { } literalContext)
        {
            throw new SyntaxException("Expected literal expression", context);
        }
        
        if (literalContext.STRING_LITERAL() is { } stringLiteral)
        {
            return new StringConstant
            {
                Compiler = this,
                Context = literalContext,
                Value = stringLiteral.Symbol.Text.Substring(1, stringLiteral.Symbol.Text.Length - 2) // Remove quotes
            };
        }
        
        if (literalContext.DECIMAL_LITERAL() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid decimal literal", literalContext);
            }

            var decimalPlaces = DecimalDataType.DEFAULT_DECIMAL_PLACES;
            
            if (decimalLiteral.Symbol.Text.Split('.').LastOrDefault() is { } decimalPart)
            {
                decimalPlaces = decimalPart.Length;
            }

            return new DecimalConstant
            {
                Compiler = this,
                Context = literalContext,
                Value = result,
                DecimalPlaces = decimalPlaces
            };
        }
        
        if (literalContext.INTEGER_LITERAL() is { } integerLiteral)
        {
            if (!int.TryParse(integerLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid integer literal", literalContext);
            }
            
            return new IntegerConstant
            {
                Compiler = this,
                Context = literalContext,
                Value = result
            };
        }

        if (literalContext.boolean_literal() is { } booleanLiteral)
        {
            var value = booleanLiteral.GetText() == "true";

            return new BooleanConstant
            {
                Compiler = this,
                Context = literalContext,
                Value = value
            };
        }
        
        if (literalContext.array_creation() is { } arrayCreation)
        {
            return VisitArray_creation(arrayCreation);
        }
        
        if (literalContext.object_creation() is { } objectCreation)
        {
            return VisitObject_creation(objectCreation);
        }
        
        throw new UnreachableException();
    }
}