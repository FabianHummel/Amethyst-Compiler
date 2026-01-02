using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public override Color VisitPreprocessorColorLiteral(AmethystParser.PreprocessorColorLiteralContext context)
    {
        return (Color)Visit(context.GetChild(0))!;
    }

    public override Color VisitPreprocessorRgbaLiteral(AmethystParser.PreprocessorRgbaLiteralContext context)
    {
        var red = ValidateColorComponent(context.r, "Red", context);
        var green = ValidateColorComponent(context.g, "Green", context);
        var blue = ValidateColorComponent(context.b, "Blue", context);
        var alpha = ValidateColorComponent(context.a, "Alpha", context);
        return new Color(red, green, blue, alpha);
    }
    
    private byte ValidateColorComponent(AmethystParser.PreprocessorExpressionContext expressionContext, string componentName, ParserRuleContext context)
    {
        if (VisitPreprocessorExpression(expressionContext) is not AbstractNumericPreprocessorValue numericValue)
        {
            throw new SyntaxException($"{componentName} component of RGBA color must be a numeric value.", context);
        }
        
        if (numericValue.AsInteger is < 0 or > 255)
        {
            throw new SyntaxException($"{componentName} component of RGBA color must be between 0 and 255.", context);
        }
        
        return (byte)numericValue.AsInteger;
    }
}