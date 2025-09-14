using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRecordSelectorCreation(AmethystParser.RecordSelectorCreationContext context)
    {
        return VisitRecordSelectorList(context.recordSelectorList());
    }

    public override SelectorQueryResult VisitRecordSelectorList(AmethystParser.RecordSelectorListContext context)
    {
        var records = new List<string>();
        var containsRuntimeValues = false;

        if (context.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
        {
            var result = VisitPreprocessorYieldingStatement(preprocessorYieldingStatementContext);
            // TODO: Validate that the result is a record selector and return it
        }
        
        foreach (var recordSelectorElementContext in context.recordSelectorElement())
        {
            var queryResult = VisitRecordSelectorElement(recordSelectorElementContext);
            containsRuntimeValues |= queryResult.ContainsRuntimeValues;
            records.Add(queryResult.ToString());
        }

        return new SelectorQueryResult("records", $"scores={{{string.Join(",", records)}}}", containsRuntimeValues);
    }

    public override SelectorQueryResult VisitRecordSelectorElement(AmethystParser.RecordSelectorElementContext context)
    {
        var identifier = context.IDENTIFIER();
        var recordName = identifier.GetText();

        if (!Scope.TryGetSymbol(recordName, out var symbol) || symbol is not Record record)
        {
            throw new SyntaxException($"The record '{recordName}' does not exist in the current context.", context);
        }

        SelectorQueryResult queryResult;

        if (context.expression() is { } expressionContext)
        {
            queryResult = VisitNumericSelector(record.Name, expressionContext, allowDecimals: false);
        }
        else if (context.rangeExpression() is { } rangeExpressionContext)
        {
            queryResult = VisitRangeSelector(record.Name, rangeExpressionContext, allowDecimals: false);
        }
        else
        {
            throw new SyntaxException("Expected numeric or range expression.", context);
        }

        return queryResult;
    }
}