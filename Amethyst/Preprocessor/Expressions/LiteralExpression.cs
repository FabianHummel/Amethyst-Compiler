using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Parses a preprocessor literal expression and returns an instance of
    /// <see cref="AbstractPreprocessorValue" />.</summary>
    /// <exception cref="SyntaxException">The literal is invalid.</exception>
    /// <seealso cref="VisitLiteralExpression" />
    public override AbstractPreprocessorValue VisitPreprocessorLiteralExpression(AmethystParser.PreprocessorLiteralExpressionContext context)
    {
        var literalContext = context.preprocessorLiteral();
        
        if (literalContext.preprocessorStringLiteral() is { } stringLiteralContext)
        {
            return (PreprocessorString)Visit(stringLiteralContext)!;
        }
        
        if (literalContext.preprocessorResourceLiteral() is { } resourceLiteral)
        {
            return new PreprocessorResource
            {
                Compiler = this,
                Context = literalContext,
                Value = (Resource)Visit(resourceLiteral)!
            };
        }
        
        if (literalContext.DECIMAL_LITERAL() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException($"Invalid decimal literal '{decimalLiteral}'.", literalContext);
            }

            return new PreprocessorDecimal
            {
                Compiler = this,
                Context = literalContext,
                Value = result
            };
        }
        
        if (literalContext.INTEGER_LITERAL() is { } integerLiteral)
        {
            if (!int.TryParse(integerLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException($"Invalid integer literal '{integerLiteral}'.", literalContext);
            }
            
            return new PreprocessorInteger
            {
                Compiler = this,
                Context = literalContext,
                Value = result
            };
        }

        if (literalContext.booleanLiteral() is { } booleanLiteralContext)
        {
            var stringText = booleanLiteralContext.GetText();
            var value = stringText == "true";
            if (!value && stringText != "false")
            {
                throw new SyntaxException($"Invalid boolean literal '{stringText}'.", booleanLiteralContext);
            }

            return new PreprocessorBoolean
            {
                Compiler = this,
                Context = booleanLiteralContext,
                Value = value
            };
        }

        if (literalContext.preprocessorObjectLiteral() is { } objectLiteralContext)
        {
            return new PreprocessorObject
            {
                Compiler = this,
                Context = objectLiteralContext,
                Value = VisitPreprocessorObjectLiteral(objectLiteralContext)
            };
        }

        if (literalContext.preprocessorArrayLiteral() is { } arrayLiteralContext)
        {
            return new PreprocessorArray
            {
                Compiler = this,
                Context = arrayLiteralContext,
                Value = VisitPreprocessorArrayLiteral(arrayLiteralContext)
            };
        }

        if (literalContext.preprocessorColorLiteral() is { } colorLiteralContext)
        {
            return new PreprocessorColor
            {
                Compiler = this,
                Context = colorLiteralContext,
                Value = VisitPreprocessorColorLiteral(colorLiteralContext)
            };
        }
        
        throw new InvalidOperationException($"Invalid literal '{literalContext}'.");
    }
}