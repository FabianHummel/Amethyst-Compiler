using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractDatatype VisitType(AmethystParser.TypeContext context)
    {
        Modifier? modifier = null;
        if (context.LRBRACKET() != null)
        {
            modifier = Modifier.Array;
        }
        else if (context.LRBRACE() != null)
        {
            modifier = Modifier.Object;
        }
        else if (context.GetChild(1) is { } modifierContext)
        {
            var modifierString = modifierContext.GetText();
            throw new InvalidOperationException($"Invalid type modifier '{modifierString}'.");
        }
        
        if (context.@decimal() is { } decimalContext)
        {
            var numDecimalPlaces = DecimalDatatype.DEFAULT_DECIMAL_PLACES;

            if (decimalContext.INTEGER_LITERAL() is { } integerLiteral)
            {
                numDecimalPlaces = int.Parse(integerLiteral.Symbol.Text);
            }
            
            return new DecimalDatatype
            {
                DecimalPlaces = numDecimalPlaces,
                Modifier = modifier
            };
        }

        if (context.rawLocation() is { } rawLocationContext)
        {
            return VisitRawLocation(rawLocationContext);
        }

        BasicType basicType;
        if (context.INT() != null)
        {
            basicType = BasicType.Int;
        }
        else if (context.STRING() != null)
        {
            basicType = BasicType.String;
        }
        else if (context.BOOL() != null)
        {
            basicType = BasicType.Bool;
        }
        else if (context.ARRAY() != null)
        {
            basicType = BasicType.Array;
        }
        else if (context.OBJECT() != null)
        {
            basicType = BasicType.Object;
        }
        else if (context.ENTITY() != null)
        {
            basicType = BasicType.Entity;
        }
        else
        {
            var basicTypeString = context.GetChild(0).GetText();
            throw new InvalidOperationException($"Invalid basic type '{basicTypeString}'.");
        }
        
        return AbstractDatatype.Parse(basicType, modifier);
    }
    
    private AbstractDatatype GetOrInferTypeResult(IRuntimeValue result, AmethystParser.TypeContext? typeContext, ParserRuleContext context)
    {
        AbstractDatatype? type = null;
        
        if (typeContext != null)
        {
            type = VisitType(typeContext);
        }
        // if two types are defined, check if they match
        if (type != null && type != result.Datatype)
        {
            throw new SyntaxException($"The type '{type}' does not match the inferred type '{result.Datatype}'.", context);
        }
        // if no type is defined, we infer it
        if (type == null)
        {
            type = result.Datatype;
        }
        
        return type;
    }
}