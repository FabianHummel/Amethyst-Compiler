using System.Text;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a regular preprocessor string that is constructed of regular string literal parts parsed by 
    ///     <see cref="VisitPreprocessorRegularStringLiteralPart" />.</p>
    ///     <p>In contrast to <see cref="VisitPreprocessorInterpolatedStringLiteral" />, this type of string literal
    ///     does not support expression interpolation and treats all characters verbatim.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>"Hello World"</c> → <c>"Hello World"</c></p>
    ///     <p><c>"Verbatim {value}"</c> → <c>"Verbatim {value}"</c></p></example>
    /// <seealso cref="VisitPreprocessorInterpolatedStringLiteral" />
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
    
    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a single regular preprocessor string literal part that treats all characters verbatim.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitPreprocessorInterpolatedStringLiteralPart" />
    public override string VisitPreprocessorRegularStringLiteralPart(AmethystParser.PreprocessorRegularStringLiteralPartContext context)
    {
        return context.REGULAR_STRING_CONTENT().GetText();
    }

    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns an interpolated preprocessor string that is constructed of string literal parts parsed by 
    ///     <see cref="VisitPreprocessorInterpolatedStringLiteralPart" />. Such part may be an interpolated literal
    ///     denoted by surrounding braces over an expression.</p>
    ///     <p>In contrast to <see cref="VisitPreprocessorRegularStringLiteral" />, this type of string literal
    ///     allows for interpolated expression.</p>
    ///     <p>Interpolations may also be nested, so syntax like this: <c>$"Outer {$"Middle {"Inner"}"}"</c>
    ///     is allowed. Also notice that nested strings don't lead to nesting issues and are treated pairwise.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>$"Hello World"</c> → <c>"Hello World"</c></p>
    ///     <p><c>$"Interpolated: {value}"</c> → <c>"Interpolated: 123"</c></p></example>
    /// <seealso cref="VisitPreprocessorInterpolatedStringLiteral" />
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

    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a single preprocessor string literal part that may be an interpolated value.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitPreprocessorRegularStringLiteralPart" />
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