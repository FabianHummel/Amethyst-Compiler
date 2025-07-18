using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitPrimary_expression(AmethystParser.Primary_expressionContext context)
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
        if (context is AmethystParser.Call_expressionContext callExpression)
        {
            return VisitCall_expression(callExpression);
        }
        if (context is AmethystParser.Identifier_expressionContext identifierExpression)
        {
            return VisitIdentifier_expression(identifierExpression);
        }
        if (context is AmethystParser.Indexed_accessContext indexedAccess)
        {
            return VisitIndexed_access(indexedAccess);
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