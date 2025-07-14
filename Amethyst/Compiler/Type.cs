using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override DataType VisitType(AmethystParser.TypeContext context)
    {
        Modifier? modifier = null;
        
        if (context.GetChild(1) is { } modifierContext)
        {
            modifier = modifierContext.GetText() switch
            {
                "[]" => Modifier.Array,
                "{}" => Modifier.Object,
                _ => throw new SyntaxException("Expected modifier.", context)
            };
        }
        
        if (context.@decimal() is { } decimalContext)
        {
            var numDecimalPlaces = DecimalDataType.DEFAULT_DECIMAL_PLACES;

            if (decimalContext.Integer_Literal() is { } integerLiteral)
            {
                numDecimalPlaces = int.Parse(integerLiteral.Symbol.Text);
            }
            
            return new DecimalDataType
            {
                BasicType = BasicType.Dec,
                DecimalPlaces = numDecimalPlaces,
                Modifier = modifier
            };
        }

        var basicType = context.GetChild(0).GetText() switch
        {
            "int" => BasicType.Int,
            "string" => BasicType.String,
            "bool" => BasicType.Bool,
            "array" => BasicType.Array,
            "object" => BasicType.Object,
            _ => throw new SyntaxException("Expected basic type.", context)
        };
        
        return new DataType
        {
            BasicType = basicType,
            Modifier = modifier
        };
    }
}