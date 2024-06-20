using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Assignment()
    {
        var expr = Or();

        if (Match(TokenType.EQUAL))
        {
            var equals = Previous();
            var value = Assignment();

            if (expr is Expr.Variable variable)
            {
                return new Expr.Assign
                {
                    Name = variable.Name,
                    Value = value
                };
            }

            throw new SyntaxException("Invalid assignment target", equals.Line, Context.SourcePath);
        }

        return expr;
    }
}