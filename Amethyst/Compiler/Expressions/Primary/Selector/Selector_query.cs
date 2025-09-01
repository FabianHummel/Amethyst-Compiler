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
    
    public new SelectorQueryResult VisitSelector_query(AmethystParser.Selector_queryContext context)
    {
        if (context is AmethystParser.Expression_selectorContext expressionSelectorContext)
        {
            if (expressionSelectorContext.IDENTIFIER() is not { } identifier)
            {
                throw new SyntaxException("Expected identifier.", context);
            }
            
            if (expressionSelectorContext.expression() is not { } expressionContext)
            {
                throw new SyntaxException("Expected expression.", context);
            }

            var queryKey = identifier.GetText();

            if (queryKey is "x" or "y" or "z" or "dx" or "dy" or "dz" or "distance" or "x_rotation" or "y_rotation")
            {
                return VisitNumeric_selector(queryKey, expressionContext, allowDecimals: true);
            }

            if (queryKey is "tag")
            {
                return VisitString_selector(queryKey, expressionContext);
            }

            if (queryKey is "tags")
            {
                return VisitTags_selector(expressionContext);
            }
        }

        if (context is AmethystParser.Range_selectorContext rangeSelectorContext)
        {
            if (rangeSelectorContext.IDENTIFIER() is not { } identifier)
            {
                throw new SyntaxException("Expected identifier.", context);
            }
            
            if (rangeSelectorContext.range_expression() is not { } rangeExpressionContext)
            {
                throw new SyntaxException("Expected range expression.", context);
            }
            
            var queryKey = identifier.GetText();

            if (queryKey is "distance" or "x_rotation" or "y_rotation")
            {
                return VisitRange_selector(queryKey, rangeExpressionContext, allowDecimals: true);
            }
        }

        if (context is AmethystParser.Records_selectorContext recordsSelectorContext)
        {
            if (recordsSelectorContext.IDENTIFIER() is not { } identifier)
            {
                throw new SyntaxException("Expected identifier.", context);
            }
            
            if (recordsSelectorContext.record_selector_list() is not { } recordSelectorListContext)
            {
                throw new SyntaxException("Expected record list.", context);
            }
            
            var queryKey = identifier.GetText();

            if (queryKey is "records")
            {
                return VisitRecord_selector_list(recordSelectorListContext);
            }
        }

        throw new SyntaxException("Invalid selector.", context);
    }
    
    public SelectorQueryResult VisitNumeric_selector(string queryKey, AmethystParser.ExpressionContext context, bool allowDecimals)
    {
        if (VisitExpression(context) is not { } result)
        {
            throw new SyntaxException("Expected expression.", context);
        }
        
        if (!allowDecimals && result is DecimalConstant or DecimalResult)
        {
            throw new SyntaxException("Unexpected decimal value.", context);
        }

        var queryResult = result.ToTargetSelectorString();

        return new SelectorQueryResult(queryKey, $"{queryKey}={queryResult}", result is RuntimeValue);
    }
    
    public SelectorQueryResult VisitRange_selector(string queryKey, AmethystParser.Range_expressionContext context, bool allowDecimals)
    {
        if (VisitRange_expression(context, allowDecimals) is { } rangeExpressionResult)
        {
            return new SelectorQueryResult(queryKey, $"{queryKey}={rangeExpressionResult}", rangeExpressionResult.ContainsRuntimeValues);
        }
        
        throw new SyntaxException("Expected range expression.", context);
    }

    public SelectorQueryResult VisitString_selector(string queryKey, AmethystParser.ExpressionContext context)
    {
        if (VisitExpression(context) is not { } result)
        {
            throw new SyntaxException("Expected expression.", context);
        }

        var queryResult = result.ToTargetSelectorString();
        
        return new SelectorQueryResult(queryKey, $"{queryKey}={queryResult}", result is RuntimeValue);
    }
}