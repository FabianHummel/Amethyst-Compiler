using System.Text;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override PreprocessorString VisitPreprocessorRegularStringLiteral(AmethystParser.PreprocessorRegularStringLiteralContext context)
    {
        var str = new StringBuilder();

        foreach (var stringLiteralPartContext in context.preprocessorRegularStringLiteralPart())
        {
            var partValue = VisitPreprocessorRegularStringLiteralPart(stringLiteralPartContext);
            str.Append(partValue);
        }

        return new PreprocessorString
        {
            Compiler = this,
            Context = context,
            Value = str.ToString()
        };
    }
    
    public override string VisitPreprocessorRegularStringLiteralPart(AmethystParser.PreprocessorRegularStringLiteralPartContext context)
    {
        return context.REGULAR_STRING_CONTENT().GetText();
    }

    public override PreprocessorString VisitPreprocessorInterpolatedStringLiteral(AmethystParser.PreprocessorInterpolatedStringLiteralContext context)
    {
        var str = new StringBuilder();

        foreach (var stringLiteralPartContext in context.preprocessorInterpolatedStringLiteralPart())
        {
            var partValue = VisitPreprocessorInterpolatedStringLiteralPart(stringLiteralPartContext);
            str.Append(partValue);
        }

        return new PreprocessorString
        {
            Compiler = this,
            Context = context,
            Value = str.ToString()
        };
    }

    public override string VisitPreprocessorInterpolatedStringLiteralPart(AmethystParser.PreprocessorInterpolatedStringLiteralPartContext context)
    {
        if (context.INTERP_STRING_CONTENT() is { } stringContent)
        {
            return stringContent.GetText();
        }
        
        if (context.preprocessorExpression() is { } expressionContext)
        {
            var result = VisitPreprocessorExpression(expressionContext);
            return result.AsString;
        }
        
        throw new InvalidOperationException($"Unknown string literal part '{context.GetText()}'");
    }
}