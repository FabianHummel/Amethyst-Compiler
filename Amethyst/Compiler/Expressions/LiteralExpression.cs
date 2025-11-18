using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Creates new constant values by a literal value, e.g. values directly embedded into the
    ///     source code.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">The literal is unknown or not in the correct format.</exception>
    /// <exception cref="InvalidOperationException">The literal is not yet supported by the compiler.</exception>
    public override AbstractValue VisitLiteralExpression(AmethystParser.LiteralExpressionContext context)
    {
        var literalContext = context.literal();
        
        if (literalContext.STRING_LITERAL() is { } stringLiteral)
        {
            return new ConstantString
            {
                Compiler = this,
                Context = literalContext,
                Value = stringLiteral.GetText()[1..^1]
            };
        }
        
        if (literalContext.DECIMAL_LITERAL() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.GetText(), out var result))
            {
                throw new SyntaxException($"Invalid decimal literal '{decimalLiteral.GetText()}'.", literalContext);
            }

            var decimalPlaces = DecimalDatatype.DEFAULT_DECIMAL_PLACES;
            
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

        if (literalContext.rawLocation() is { } rawLocationContext)
        {
            var rawLocation = VisitRawLocation(rawLocationContext);
            return new RawLocation
            {
                Compiler = this,
                Context = literalContext,
                Location = rawLocation
            };
        }

        if (literalContext.selectorCreation() is { } selectorCreationContext)
        {
            return VisitSelectorCreation(selectorCreationContext);
        }
        
        throw new InvalidOperationException($"Invalid literal '{literalContext.GetText()}'.");
    }
}