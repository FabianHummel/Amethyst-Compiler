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
    
    public new SelectorQueryResult VisitSelectorQuery(AmethystParser.SelectorQueryContext context)
    {
        if (context is AmethystParser.ExpressionSelectorContext expressionSelectorContext)
        {
            var identifier = expressionSelectorContext.IDENTIFIER();
            var queryKey = identifier.GetText();
            
            var expressionContext = expressionSelectorContext.expression();

            if (queryKey is "x" or "y" or "z" or "dx" or "dy" or "dz" or "distance" or "x_rotation" or "y_rotation")
            {
                return VisitNumericSelector(queryKey, expressionContext, allowDecimals: true);
            }

            if (queryKey is "tag")
            {
                return VisitStringSelector(queryKey, expressionContext);
            }

            if (queryKey is "tags")
            {
                return VisitTags_selector(expressionContext);
            }
        }

        if (context is AmethystParser.RangeSelectorContext rangeSelectorContext)
        {
            var identifier = rangeSelectorContext.IDENTIFIER();
            var queryKey = identifier.GetText();
            
            if (queryKey is "distance" or "x_rotation" or "y_rotation")
            {
                var rangeExpressionContext = rangeSelectorContext.rangeExpression();
                return VisitRangeSelector(queryKey, rangeExpressionContext, allowDecimals: true);
            }
        }

        if (context is AmethystParser.RecordsSelectorContext recordsSelectorContext)
        {
            var identifier = recordsSelectorContext.IDENTIFIER();
            var queryKey = identifier.GetText();

            if (queryKey is "records")
            {
                var recordSelectorListContext = recordsSelectorContext.recordSelectorList();
                return VisitRecordSelectorList(recordSelectorListContext);
            }
        }

        throw new SyntaxException("Invalid selector.", context);
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