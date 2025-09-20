using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public class SelectorQueryResult
    {
        public string QueryKey { get; }
        public string QueryString { get; }
        public bool ContainsRuntimeValues { get; }
        public AbstractResult? LimitExpression { get; }

        public SelectorQueryResult(string queryKey, string queryString, bool containsRuntimeValues, AbstractResult? limitExpression = null)
        {
            QueryKey = queryKey;
            QueryString = queryString;
            ContainsRuntimeValues = containsRuntimeValues;
            LimitExpression = limitExpression;
        }

        public override string ToString()
        {
            return QueryString;
        }
    }
    
    public SelectorQueryResult VisitNumericSelector(string queryKey, AmethystParser.ExpressionContext context, bool allowDecimals)
    {
        var result = VisitExpression(context);
        
        if (!allowDecimals && result is DecimalConstant or DecimalResult)
        {
            throw new SyntaxException("Unexpected decimal value.", context);
        }

        var queryResult = result.ToTargetSelectorString();

        return new SelectorQueryResult(queryKey, $"{queryKey}={queryResult}", result is RuntimeValue);
    }
    
    public SelectorQueryResult VisitRangeSelector(string queryKey, AmethystParser.RangeExpressionContext context, bool allowDecimals)
    {
        if (VisitRangeExpression(context, allowDecimals) is { } rangeExpressionResult)
        {
            return new SelectorQueryResult(queryKey, $"{queryKey}={rangeExpressionResult}", rangeExpressionResult.ContainsRuntimeValues);
        }
        
        throw new SyntaxException("Expected range expression.", context);
    }

    public SelectorQueryResult VisitStringSelector(string queryKey, AmethystParser.ExpressionContext context)
    {
        var result = VisitExpression(context);
        
        var queryResult = result.ToTargetSelectorString();
        
        return new SelectorQueryResult(queryKey, $"{queryKey}={queryResult}", result is RuntimeValue);
    }
}