using Amethyst.Language;

namespace Amethyst;

/// <summary>
/// Provides access to members of a type via dot notation.
/// </summary>
public interface IMemberAccess
{
    AbstractResult GetMember(string memberName);
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
        
        if (context.IDENTIFIER() is not { } identifier)
        {
            throw new SyntaxException("Expected member identifier.", context);
        }
        
        var memberName = identifier.GetText();
        
        if (result is not IMemberAccess memberAccess)
        {
            throw new SyntaxException($"Type '{result.DataType}' does not support member access.", primaryExpressionContext);
        }

        AbstractResult memberResult;
        try
        {
            memberResult = memberAccess.GetMember(memberName);
        }
        catch (SemanticException e)
        {
            throw new SyntaxException(e.Message, context);
        }
        
        return memberResult;
    }
}