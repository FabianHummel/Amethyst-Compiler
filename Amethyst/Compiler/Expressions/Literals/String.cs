using System.Text;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
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

    public override string VisitRegularStringLiteralPart(AmethystParser.RegularStringLiteralPartContext context)
    {
        return context.REGULAR_STRING_CONTENT().GetText();
    }

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
                str.Append(runtimeValue.ToMacroPlaceholder());
                containsRuntimeValues = true;
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

        string mcFunctionPath;
        RuntimeString value;
        using (this.EvaluateScoped("_create_string"))
        {
            mcFunctionPath = Scope.McFunctionPath;
            var location = Location.Storage(++StackPointer);
            this.AddCode($"$storage modify {location} set value \"{str}\"");
            value = new RuntimeString
            {
                Compiler = this,
                Context = context,
                Location = location,
                IsTemporary = true
            };
        }
        
        this.AddCode($"function {mcFunctionPath} with storage amethyst:");
        
        return value;
    }
    
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