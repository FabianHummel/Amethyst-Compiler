using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractDatatype VisitType(AmethystParser.TypeContext context)
    {
        Modifier? modifier = null;
        if (context.GetChild(1) is { } modifierContext)
        {
            var modifierString = modifierContext.GetText();
            modifier = modifierString switch
            {
                "[]" => Modifier.Array,
                "{}" => Modifier.Object,
                _ => throw new SyntaxException($"Invalid type modifier '{modifierString}'.", context)
            };
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

        var basicTypeString = context.GetChild(0).GetText();
        var basicType = basicTypeString switch
        {
            "int" => BasicType.Int,
            "string" => BasicType.String,
            "bool" => BasicType.Bool,
            "array" => BasicType.Array,
            "object" => BasicType.Object,
            _ => throw new SyntaxException($"Invalid basic type '{basicTypeString}'.", context)
        };
        
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