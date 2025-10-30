using Amethyst.Language;

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
        
        if (literalContext.STRING_LITERAL() is { } stringLiteral)
        {
            return new PreprocessorString
            {
                Compiler = this,
                Context = literalContext,
                Value = stringLiteral.GetText()[1..^1]
            };
        }
        
        if (literalContext.RESOURCE_LITERAL() is { } resourceLiteral)
        {
            return new PreprocessorResource
            {
                Compiler = this,
                Context = literalContext,
                Value = resourceLiteral.GetText()[1..^1]
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

        if (literalContext.booleanLiteral() is { } booleanLiteral)
        {
            var value = booleanLiteral.GetText() == "true";
            if (!value && booleanLiteral.GetText() != "false")
            {
                throw new SyntaxException($"Invalid boolean literal '{booleanLiteral}'.", literalContext);
            }

            return new PreprocessorBoolean
            {
                Compiler = this,
                Context = literalContext,
                Value = value
            };
        }
        
        throw new InvalidOperationException($"Invalid literal '{literalContext}'.");
    }
}