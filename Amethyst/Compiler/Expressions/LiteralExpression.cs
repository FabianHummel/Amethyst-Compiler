using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitLiteralExpression(AmethystParser.LiteralExpressionContext context)
    {
        var literalContext = context.literal();
        
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

        if (literalContext.booleanLiteral() is { } booleanLiteral)
        {
            var value = booleanLiteral.GetText() == "true";

            return new BooleanConstant
            {
                Compiler = this,
                Context = literalContext,
                Value = value
            };
        }
        
        if (literalContext.arrayCreation() is { } arrayCreation)
        {
            return VisitArrayCreation(arrayCreation);
        }
        
        if (literalContext.objectCreation() is { } objectCreation)
        {
            return VisitObjectCreation(objectCreation);
        }
        
        throw new UnreachableException();
    }
}