using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRecordSelectorCreation(AmethystParser.RecordSelectorCreationContext context)
    {
        var recordSelectors = new List<string>();
        var containsRuntimeValues = false;

        var recordSelectorElementContexts = context.recordSelectorElement();
        ProcessRecordSelectorElements(recordSelectorElementContexts);

        return new SelectorQueryResult("records", $"scores={{{string.Join(",", recordSelectors)}}}", containsRuntimeValues);
        
        void ProcessRecordSelectorElements(IEnumerable<AmethystParser.RecordSelectorElementContext> recordSelectorElementContexts)
        {
            foreach (var recordSelectorElementContext in recordSelectorElementContexts)
            {
                if (recordSelectorElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
                {
                    var result = VisitPreprocessorYieldingStatement<AmethystParser.RecordSelectorElementContext>(preprocessorYieldingStatementContext);
                    ProcessRecordSelectorElements(result);
                }
                else if (recordSelectorElementContext.recordSelectorKvp() is { } recordSelectorKvpContext)
                {
                    var queryResult = VisitRecordSelectorKvp(recordSelectorKvpContext);
                    containsRuntimeValues |= queryResult.ContainsRuntimeValues;
                    recordSelectors.Add(queryResult.ToString());
                }
            }
        }
    }

    public override SelectorQueryResult VisitRecordSelectorKvp(AmethystParser.RecordSelectorKvpContext context)
    {
        var identifier = context.IDENTIFIER();
        var recordName = identifier.GetText();
        var symbol = GetSymbol(recordName, context);

        if (symbol is not Record record)
        {
            throw new SyntaxException($"Symbol '{recordName}' is not a record.", context);
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