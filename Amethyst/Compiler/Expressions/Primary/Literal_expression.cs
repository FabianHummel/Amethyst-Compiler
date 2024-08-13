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
            AddCode($"data modify storage amethyst: {MemoryLocation} set value {stringLiteral.Symbol.Text}");
            return new StringResult
            {
                Compiler = this,
                Location = MemoryLocation++.ToString(),
                Context = literalContext,
                IsTemporary = true
            };
        }

        if (literalContext.array_creation() is { } arrayCreation)
        {
            return VisitArray_creation(arrayCreation);
        }
        
        if (literalContext.boolean_literal() is { } booleanLiteral)
        {
            return VisitBoolean_literal(booleanLiteral);
        }
        
        if (literalContext.object_creation() is { } objectCreation)
        {
            return VisitObject_creation(objectCreation);
        }

        if (literalContext.Decimal_Literal() is { } decimalLiteral)
        {
            if (!double.TryParse(decimalLiteral.Symbol.ToString(), out var result))
            {
                throw new SyntaxException("Invalid decimal literal", literalContext);
            }
            
            var rounded = Math.Round(result, 2);
            
            AddCode($"scoreboard players set {MemoryLocation} amethyst {rounded}");

            return new DecResult
            {
                Compiler = this,
                Location = MemoryLocation++.ToString(),
                Context = literalContext,
                IsTemporary = true
            };
        }
        
        if (literalContext.Integer_Literal() is { } integerLiteral)
        {
            if (!int.TryParse(integerLiteral.Symbol.Text, out var result))
            {
                throw new SyntaxException("Invalid integer literal", literalContext);
            }
            
            AddCode($"scoreboard players set {MemoryLocation} amethyst {result}");

            return new IntResult
            {
                Compiler = this,
                Location = MemoryLocation++.ToString(),
                Context = literalContext,
                IsTemporary = true
            };
        }
        
        throw new UnreachableException();
    }
}