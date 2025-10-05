using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitLiteralExpression(AmethystParser.LiteralExpressionContext context)
    {
        var literalContext = context.literal();
        
        if (literalContext.STRING_LITERAL() is { } stringLiteral)
        {
            return new ConstantString
            {
                Compiler = this,
                Context = literalContext,
                Value = stringLiteral.Symbol.Text.Substring(1, stringLiteral.Symbol.Text.Length - 2) // Remove quotes
            };
        }
        
        if (literalContext.DECIMAL_LITERAL() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.GetText(), out var result))
            {
                throw new SyntaxException($"Invalid decimal literal '{decimalLiteral.GetText()}'.", literalContext);
            }

            var decimalPlaces = DecimalDataType.DEFAULT_DECIMAL_PLACES;
            
            if (decimalLiteral.GetText().Split('.').LastOrDefault() is { } decimalPart)
            {
                decimalPlaces = decimalPart.Length;
            }

            return new ConstantDecimal
            {
                Compiler = this,
                Context = literalContext,
                Value = result,
                DecimalPlaces = decimalPlaces
            };
        }
        
        if (literalContext.INTEGER_LITERAL() is { } integerLiteral)
        {
            if (!int.TryParse(integerLiteral.GetText(), out var result))
            {
                throw new SyntaxException($"Invalid integer literal '{integerLiteral.GetText()}'.", literalContext);
            }
            
            return new ConstantInteger
            {
                Compiler = this,
                Context = literalContext,
                Value = result
            };
        }

        if (literalContext.booleanLiteral() is { } booleanLiteral)
        {
            var value = booleanLiteral.GetText() == "true";
            if (!value && booleanLiteral.GetText() != "false")
            {
                throw new SyntaxException($"Invalid boolean literal '{booleanLiteral.GetText()}'.", literalContext);
            }

            return new ConstantBoolean
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
        
        throw new InvalidOperationException($"Invalid literal '{literalContext}'.");
    }
}