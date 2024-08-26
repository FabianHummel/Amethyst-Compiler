using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitLiteral_expression(AmethystParser.Literal_expressionContext context)
    {
        if (context.literal() is not { } literalContext)
        {
            throw new UnreachableException();
        }
        
        if (literalContext.String_Literal() is { } stringLiteral)
        {
            return new StringResult
            {
                Compiler = this,
                Context = literalContext,
                ConstantValue = stringLiteral.Symbol.Text
            };
        }
        
        if (literalContext.Decimal_Literal() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid decimal literal", literalContext);
            }

            return new DecimalResult
            {
                Compiler = this,
                Context = literalContext,
                ConstantValue = result
            };
        }
        
        if (literalContext.Integer_Literal() is { } integerLiteral)
        {
            if (!int.TryParse(integerLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid integer literal", literalContext);
            }
            
            return new IntegerResult
            {
                Compiler = this,
                Context = literalContext,
                ConstantValue = result
            };
        }

        if (literalContext.boolean_literal() is { } booleanLiteral)
        {
            var value = booleanLiteral.GetText() == "true";

            return new BooleanResult
            {
                Compiler = this,
                Context = literalContext,
                ConstantValue = value
            };
        }
        
        if (literalContext.array_creation() is { } arrayCreation)
        {
            return VisitArray_creation(arrayCreation);
        }
        
        if (literalContext.object_creation() is { } objectCreation)
        {
            return VisitObject_creation(objectCreation);
        }
        
        throw new UnreachableException();
    }
}