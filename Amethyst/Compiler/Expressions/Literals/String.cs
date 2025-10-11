using System.Text;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    // TODO: Kept for a future feature: string interpolation
    //
    // public override string VisitStringLiteral(AmethystParser.StringLiteralContext context)
    // {
    //     return context.GetText()[1..^1]; // Remove the surrounding quotes
    // }
    //
    // public string VisitStringLiteralPart(AmethystParser.StringLiteralPartContext context, out ConstantSubstitute? constantSubstitute)
    // {
    //     constantSubstitute = null;
    //     
    //     if (context.STRING_CONTENT() != null)
    //     {
    //         return context.STRING_CONTENT().GetText();
    //     }
    //     if (context.STRING_ESCAPE_SEQUENCE() != null)
    //     {
    //         var text = context.STRING_ESCAPE_SEQUENCE().GetText();
    //         return text switch
    //         {
    //             @"\n" => "\n",
    //             @"\r" => "\r",
    //             @"\t" => "\t",
    //             @"\""" => "\"",
    //             @"\\" => "\\",
    //             _ => throw new NotImplementedException($"Unknown escape sequence: {text}")
    //         };
    //     }
    //     if (context.STRING_UNICODE_ESCAPE() != null)
    //     {
    //         var text = context.STRING_UNICODE_ESCAPE().GetText();
    //         var hex = text[3..^1]; // Remove \u{ and }
    //         var codePoint = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    //         return char.ConvertFromUtf32(codePoint);
    //     }
    //
    //     if (context.expression() is { } expressionContext)
    //     {
    //         var expression = VisitExpression(expressionContext);
    //
    //         if (expression is IConstantValue constantValue)
    //         {
    //             return constantValue.ToNbtString();
    //         }
    //
    //         if (expression is IRuntimeValue runtimeValue)
    //         {
    //             constantSubstitute = new ConstantSubstitute
    //             {
    //                 Compiler = this,
    //                 Context = expressionContext,
    //                 Value = runtimeValue
    //             };
    //             
    //             return // a placeholder for function macro replacement
    //         }
    //         
    //         throw new InvalidOperationException("Expression in string literal is neither constant nor runtime");
    //     }
    //
    //     throw new InvalidOperationException($"Unknown string literal part '{context.GetText()}'");
    // }
}