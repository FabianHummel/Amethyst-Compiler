using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRecord_selector_list(AmethystParser.Record_selector_listContext context)
    {
        var records = new List<string>();
        var containsRuntimeValues = false;

        foreach (var recordSelectorElementContext in context.record_selector_element())
        {
            if (recordSelectorElementContext.IDENTIFIER() is not { } identifier)
            {
                throw new SyntaxException("Expected identifier.", recordSelectorElementContext);
            }

            var recordName = identifier.GetText();

            if (!Scope.TryGetSymbol(recordName, out var symbol) || symbol is not Record record)
            {
                throw new SyntaxException($"The record '{recordName}' does not exist in the current context.", context);
            }

            SelectorQueryResult queryResult;

            if (recordSelectorElementContext.expression() is { } expressionContext)
            {
                queryResult = VisitNumeric_selector(record.Name, expressionContext, allowDecimals: false);
            }
            else if (recordSelectorElementContext.range_expression() is { } rangeExpressionContext)
            {
                queryResult = VisitRange_selector(record.Name, rangeExpressionContext, allowDecimals: false);
            }
            else
            {
                throw new SyntaxException("Expected numeric or range expression.", context);
            }
            
            containsRuntimeValues |= queryResult.ContainsRuntimeValues;
            records.Add(queryResult.ToString());
        }

        return new SelectorQueryResult("records", $"scores={{{string.Join(",", records)}}}", containsRuntimeValues);
    }
}