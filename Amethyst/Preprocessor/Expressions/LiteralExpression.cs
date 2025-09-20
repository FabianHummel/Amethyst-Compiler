using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override PreprocessorResult VisitPreprocessorLiteralExpression(AmethystParser.PreprocessorLiteralExpressionContext context)
    {
        var literalContext = context.preprocessorLiteral();
        
        if (literalContext.STRING_LITERAL() is { } stringLiteral)
        {
            return new PreprocessorStringResult
            {
                Compiler = this,
                Context = literalContext,
                Value = stringLiteral.GetText()[1..^1] // Remove quotes
            };
        }
        
        if (literalContext.RESOURCE_LITERAL() is { } resourceLiteral)
        {
            return new PreprocessorResourceResult
            {
                Compiler = this,
                Context = literalContext,
                Value = resourceLiteral.GetText()[1..^1] // Remove backticks
            };
        }
        
        if (literalContext.DECIMAL_LITERAL() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid decimal literal", literalContext);
            }

            return new PreprocessorDecimalResult
            {
                Compiler = this,
                Context = literalContext,
                Value = result,
            };
        }
        
        if (literalContext.INTEGER_LITERAL() is { } integerLiteral)
        {
            if (!int.TryParse(integerLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid integer literal", literalContext);
            }
            
            return new PreprocessorIntegerResult
            {
                Compiler = this,
                Context = literalContext,
                Value = result
            };
        }

        if (literalContext.booleanLiteral() is { } booleanLiteral)
        {
            var value = booleanLiteral.GetText() == "true";

            return new PreprocessorBooleanResult
            {
                Compiler = this,
                Context = literalContext,
                Value = value
            };
        }
        
        throw new UnreachableException();
    }
}