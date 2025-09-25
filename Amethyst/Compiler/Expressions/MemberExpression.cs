using Amethyst.Language;

namespace Amethyst;

/// <summary>
/// Provides access to members of a type.
/// </summary>
public interface IMemberAccess
{
    AbstractValue GetMember(string memberName);
}

public partial class Compiler
{
    public override AbstractValue VisitMemberExpression(AmethystParser.MemberExpressionContext context)
    {
        var expressionContext = context.expression();
        var result = VisitExpression(expressionContext);
        
        if (result is not IMemberAccess memberAccess)
        {
            throw new SyntaxException($"Type '{result.DataType}' does not support member access.", expressionContext);
        }

        try
        {
            var identifier = context.IDENTIFIER();
            var memberName = identifier.GetText();
            return memberAccess.GetMember(memberName);
        }
        catch (SemanticException e)
        {
            throw new SyntaxException(e.Message, context);
        }
    }
}