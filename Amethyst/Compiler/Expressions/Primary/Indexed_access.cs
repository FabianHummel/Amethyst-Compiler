using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitIndexed_access(AmethystParser.Indexed_accessContext context)
    {
        if (context.primary_expression() is not { } primaryExpressionContext)
        {
            throw new SyntaxException("Expected primary expression.", context);
        }

        var result = VisitPrimary_expression(primaryExpressionContext);

        if (context.expression() is not { } expressionContext)
        {
            throw new SyntaxException("Expected index expression.", context);
        }

        var index = VisitExpression(expressionContext);

        if (result is not IIndexable indexable)
        {
            throw new SyntaxException($"Type '{result.DataType}' is not indexable.", primaryExpressionContext);
        }

        return indexable.GetIndex(index);

/*
if (index is not IntegerConstant and not IntegerResult)
{
    throw new SyntaxException("Expected integer index.", expressionContext);
}

if (result is ArrayConstantBase arrayConstantBase)
{
    if (index is IntegerConstant integerConstant)
    {
        var value = integerConstant.Value;

        if (value < -arrayConstantBase.Value.Length || value >= arrayConstantBase.Value.Length)
        {
            throw new SyntaxException($"Index {value} out of bounds for array of length {arrayConstantBase.Value.Length}.", expressionContext);
        }

        if (value < 0)
        {
            value += arrayConstantBase.Value.Length;
        }

        return arrayConstantBase.Value[value];
    }

    // if the index is a runtime value, convert the array constant
    // to a runtime value as well and continue the evaluation.
    result = arrayConstantBase.ToRuntimeValue();
}

if (result is ArrayBase arrayBase)
{
    if (index is IntegerConstant integerConstant)
    {
        AddCode("");
    }
}
else
{
    throw new SyntaxException("Expected array.", primaryExpressionContext);
}
*/
    }
}