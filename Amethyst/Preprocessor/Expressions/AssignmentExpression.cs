using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Assigns a value to a preprocessor variable.</p><p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">Thrown when an invalid operator is used.</exception>
    /// <seealso cref="VisitAssignmentExpression" />
    public override object? VisitPreprocessorAssignmentExpression(AmethystParser.PreprocessorAssignmentExpressionContext context)
    {
        var expressionContexts = context.preprocessorExpression();
        var lhs = VisitPreprocessorExpression(expressionContexts[0]);
        var rhs = VisitPreprocessorExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<AssignmentOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        if (op == AssignmentOperator.ASSIGN)
        {
            AssignPreprocessorValue(lhs, rhs);
            return null;
        }
        
        var result = OperationRegistry.Resolve<AbstractPreprocessorValue, AssignmentOperator>(this, context, op, lhs, rhs);
        
        AssignPreprocessorValue(lhs, result);
        return null;
    }

    private static void AssignPreprocessorValue(AbstractPreprocessorValue target, AbstractPreprocessorValue source)
    {
        target.SetValue(source.AbstractValue);
    }
}