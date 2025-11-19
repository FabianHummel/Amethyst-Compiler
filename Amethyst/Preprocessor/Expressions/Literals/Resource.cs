using System.Text;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a <see cref="Resource" /> that is constructed of regular resource literal parts parsed by 
    ///     <see cref="VisitPreprocessorRegularResourceLiteralPart" />.</p>
    ///     <p>In contrast to <see cref="VisitPreprocessorInterpolatedResourceLiteral" />, this type of resource literal
    ///     does not support expression interpolation and treats all characters verbatim.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>`minecraft:path/to/resource`</c> → <c>`minecraft:path/to/resource`</c></p>
    ///     <p><c>`minecraft:verbatim/{resource}`</c> → <c>`minecraft:verbatim/{resource}`</c></p></example>
    /// <seealso cref="VisitPreprocessorInterpolatedResourceLiteral" />
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
    
    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a single regular preprocessor resource literal part that treats all characters verbatim.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitPreprocessorInterpolatedResourceLiteralPart" />
    public override string VisitPreprocessorRegularResourceLiteralPart(AmethystParser.PreprocessorRegularResourceLiteralPartContext context)
    {
        return context.REGULAR_RESOURCE_CONTENT().GetText();
    }

    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a <see cref="Resource" /> that is constructed of resource literal parts parsed by 
    ///     <see cref="VisitPreprocessorInterpolatedResourceLiteralPart" />. Such part may be an interpolated literal
    ///     denoted by surrounding braces over an expression.</p>
    ///     <p>In contrast to <see cref="VisitPreprocessorRegularResourceLiteral" />, this type of resource literal
    ///     allows for interpolated expression.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>`minecraft:path/to/resource`</c> → <c>`minecraft:path/to/resource`</c></p>
    ///     <p><c>`minecraft:interpolated/{resource}`</c> → <c>`minecraft:interpolated/resource_123`</c></p></example>
    /// <seealso cref="VisitPreprocessorInterpolatedResourceLiteral" />
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

    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a single preprocessor resource literal part that may be an interpolated value.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitPreprocessorRegularResourceLiteralPart" />
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