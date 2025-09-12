using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRecordSelectorList(AmethystParser.RecordSelectorListContext context)
    {
        var records = new List<string>();
        var containsRuntimeValues = false;

        foreach (var recordSelectorElementContext in context.recordSelectorElement())
        {
            var identifier = recordSelectorElementContext.IDENTIFIER();
            var recordName = identifier.GetText();

            if (!Scope.TryGetSymbol(recordName, out var symbol) || symbol is not Record record)
            {
                throw new SyntaxException($"The record '{recordName}' does not exist in the current context.", context);
            }

            SelectorQueryResult queryResult;

            if (recordSelectorElementContext.expression() is { } expressionContext)
            {
                queryResult = VisitNumericSelector(record.Name, expressionContext, allowDecimals: false);
            }
            else if (recordSelectorElementContext.rangeExpression() is { } rangeExpressionContext)
            {
                queryResult = VisitRangeSelector(record.Name, rangeExpressionContext, allowDecimals: false);
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