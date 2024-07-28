using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitPrimary(AmethystParser.PrimaryContext context)
    {
        if (context is AmethystParser.Literal_expressionContext literalExpression)
        {
            return VisitLiteral_expression(literalExpression);
        }
        if (context is AmethystParser.Group_expressionContext groupExpression)
        {
            return VisitGroup_expression(groupExpression);
        }
        if (context is AmethystParser.Selector_expressionContext selectorExpression)
        {
            return VisitSelector_expression(selectorExpression);
        }
        if (context is AmethystParser.Member_accessContext memberAccess)
        {
            return VisitMember_access(memberAccess);
        }
        if (context is AmethystParser.Function_call_expressionContext functionCallExpression)
        {
            return VisitFunction_call_expression(functionCallExpression);
        }
        if (context is AmethystParser.Identifier_expressionContext identifierExpression)
        {
            return VisitIdentifier_expression(identifierExpression);
        }
        if (context is AmethystParser.Indexed_accessContext indexedAccess)
        {
            return VisitIndexed_access(indexedAccess);
        }
        if (context is AmethystParser.Object_creation_expressionContext objectCreationExpression)
        {
            return VisitObject_creation_expression(objectCreationExpression);
        }
        if (context is AmethystParser.Array_creation_expressionContext arrayCreationExpression)
        {
            return VisitArray_creation_expression(arrayCreationExpression);
        }
        if (context is AmethystParser.Post_incrementContext postIncrement)
        {
            return VisitPost_increment(postIncrement);
        }
        if (context is AmethystParser.Post_decrementContext postDecrement)
        {
            return VisitPost_decrement(postDecrement);
        }
        
        throw new UnreachableException();
    }
}