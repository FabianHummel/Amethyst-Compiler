using Amethyst.Language;

namespace Amethyst;

/// <summary>
/// Provides access to members of a type via dot notation.
/// </summary>
public interface IMemberAccess
{
    AbstractResult GetMember(string memberName, AmethystParser.IdentifierContext identifierContext);
}

public partial class Compiler
{
    public override AbstractResult VisitMember_access(AmethystParser.Member_accessContext context)
    {
        if (context.primary_expression() is not { } primaryExpressionContext)
        {
            throw new SyntaxException("Expected primary expression.", context);
        }
        
        var result = VisitPrimary_expression(primaryExpressionContext);
        
        if (context.identifier() is not { } identifierContext)
        {
            throw new SyntaxException("Expected member identifier.", context);
        }
        
        var memberName = identifierContext.GetText();
        
        if (result is not IMemberAccess memberAccess)
        {
            throw new SyntaxException($"Type '{result.DataType}' does not support member access.", primaryExpressionContext);
        }

        return memberAccess.GetMember(memberName, identifierContext);
    }
}