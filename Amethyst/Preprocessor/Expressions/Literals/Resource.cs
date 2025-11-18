using System.Text;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override Resource VisitPreprocessorRegularResourceLiteral(AmethystParser.PreprocessorRegularResourceLiteralContext context)
    {
        var str = new StringBuilder();

        foreach (var resourceLiteralPartContext in context.preprocessorRegularResourceLiteralPart())
        {
            var partValue = VisitPreprocessorRegularResourceLiteralPart(resourceLiteralPartContext);
            str.Append(partValue);
        }

        return new Resource(str.ToString());
    }
    
    public override string VisitPreprocessorRegularResourceLiteralPart(AmethystParser.PreprocessorRegularResourceLiteralPartContext context)
    {
        return context.REGULAR_RESOURCE_CONTENT().GetText();
    }

    public override Resource VisitPreprocessorInterpolatedResourceLiteral(AmethystParser.PreprocessorInterpolatedResourceLiteralContext context)
    {
        var str = new StringBuilder();

        foreach (var resourceLiteralPartContext in context.preprocessorInterpolatedResourceLiteralPart())
        {
            var partValue = VisitPreprocessorInterpolatedResourceLiteralPart(resourceLiteralPartContext);
            str.Append(partValue);
        }

        return new Resource(str.ToString());
    }

    public override string VisitPreprocessorInterpolatedResourceLiteralPart(AmethystParser.PreprocessorInterpolatedResourceLiteralPartContext context)
    {
        if (context.INTERP_RESOURCE_CONTENT() is { } resourceContent)
        {
            return resourceContent.GetText();
        }
        
        if (context.preprocessorExpression() is { } expressionContext)
        {
            var result = VisitPreprocessorExpression(expressionContext);
            return result.AsString;
        }
        
        throw new InvalidOperationException($"Unknown resource literal part '{context.GetText()}'");
    }
}