using System.Text;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns a regular string that is constructed of regular string literal parts parsed by
    ///     <see cref="VisitRegularStringLiteralPart" />.</p>
    ///     <p>In contrast to <see cref="VisitInterpolatedStringLiteral" />, this type of string literal
    ///     does not support expression interpolation and treats all characters verbatim.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>"Hello World"</c> → <c>"Hello World"</c></p>
    ///     <p><c>"Verbatim {value}"</c> → <c>"Verbatim {value}"</c></p></example>
    /// <seealso cref="VisitInterpolatedStringLiteral" />
    public override ConstantString VisitRegularStringLiteral(AmethystParser.RegularStringLiteralContext context)
    {
        var str = new StringBuilder();
        foreach (var stringLiteralPartContext in context.regularStringLiteralPart())
        {
            var partValue = VisitRegularStringLiteralPart(stringLiteralPartContext);
            str.Append(partValue);
        }

        return new ConstantString
        {
            Compiler = this,
            Context = context,
            Value = str.ToString()
        };
    }

    /// <inheritdoc />
    /// <summary><p>Returns a single regular string literal part that treats all characters verbatim.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitInterpolatedStringLiteralPart" />
    public override string VisitRegularStringLiteralPart(AmethystParser.RegularStringLiteralPartContext context)
    {
        return context.REGULAR_STRING_CONTENT().GetText();
    }

    /// <inheritdoc />
    /// <summary>
    ///     <p>Returns an interpolated string that is constructed of string literal parts parsed by
    ///     <see cref="VisitInterpolatedStringLiteralPart" />. Such part may be an interpolated literal
    ///     denoted by surrounding braces over an expression.</p>
    ///     <p>In contrast to <see cref="VisitRegularStringLiteral" />, this type of string literal allows
    ///     for interpolated expression.</p>
    ///     <p>Interpolations may also be nested, so syntax like this:
    ///     <c>$"Outer {$"Middle {"Inner"}"}"</c> is allowed. Also notice that nested strings don't lead to
    ///     nesting issues and are treated pairwise.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>$"Hello World"</c> → <c>"Hello World"</c></p>
    ///     <p><c>$"Interpolated: {value}"</c> → <c>"Interpolated: 123"</c></p></example>
    /// <seealso cref="VisitInterpolatedStringLiteral" />
    public override AbstractString VisitInterpolatedStringLiteral(AmethystParser.InterpolatedStringLiteralContext context)
    {
        var str = new StringBuilder();
        var containsRuntimeValues = false;
        
        foreach (var stringLiteralPartContext in context.interpolatedStringLiteralPart())
        {
            var result = VisitInterpolatedStringLiteralPart(stringLiteralPartContext);
        
            if (result is IConstantValue constantValue)
            {
                str.Append(constantValue.AsString);
            }
            
            if (result is IRuntimeValue runtimeValue)
            {
                containsRuntimeValues = true;
                
                string mcFunctionPath;
                using (this.EvaluateScoped("_use_text_display"))
                {
                    mcFunctionPath = Scope.McFunctionPath;

                    var storageRuntimeValue = runtimeValue.EnsureInStorage();
                    var textComponent = storageRuntimeValue.Location.ToTextComponent();
                    this.AddCode($"data modify entity @s text set value {textComponent}");
                
                    var location = Location.Storage(++StackPointer);
                    this.AddCode($"execute in minecraft:overworld run data modify storage {location} set from entity @s text");

                    str.Append(location.ToMacroPlaceholder());
                
                    this.AddCode("kill @s");
                }
                
                this.AddCode($"execute summon minecraft:text_display run function {mcFunctionPath}");
            }
        }
        
        if (!containsRuntimeValues)
        {
            return new ConstantString
            {
                Compiler = this,
                Context = context,
                Value = str.ToString()
            };
        }
        
        string mcFunctionPath2;
        RuntimeString value;
        using (this.EvaluateScoped("_create_string"))
        {
            mcFunctionPath2 = Scope.McFunctionPath;
            var location = Location.Storage(++StackPointer);
            this.AddCode($"$data modify storage {location} set value \"{str}\"");
            value = new RuntimeString
            {
                Compiler = this,
                Context = context,
                Location = location,
                IsTemporary = true
            };
        }
        
        this.AddCode($"function {mcFunctionPath2} with storage amethyst:");
        
        return value;
    }

    /// <inheritdoc />
    /// <summary><p>Returns a single string literal part that may be an interpolated value.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitRegularStringLiteralPart" />
    public override AbstractValue VisitInterpolatedStringLiteralPart(AmethystParser.InterpolatedStringLiteralPartContext context)
    {
        if (context.INTERP_STRING_CONTENT() is { } stringContent)
        {
            return new ConstantString
            {
                Compiler = this,
                Context = context,
                Value = stringContent.GetText()
            };
        }

        if (context.expression() is { } expressionContext)
        {
            return VisitExpression(expressionContext);
        }

        throw new InvalidOperationException($"Unknown string literal part '{context.GetText()}'");
    }
}