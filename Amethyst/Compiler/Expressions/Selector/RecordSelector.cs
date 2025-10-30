using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRecordSelector(AmethystParser.RecordSelectorContext context)
    {
        var identifier = context.IDENTIFIER();
        var queryKey = identifier.GetText();

        if (queryKey is "records")
        {
            var recordSelectorCreationContext = context.recordSelectorCreation();
            return VisitRecordSelectorCreation(recordSelectorCreationContext);
        }
        
        throw new SyntaxException($"Invalid query key '{queryKey}' for record selector.", context);
    }

    /// <summary><p>Creates a record selector query result from the given context.</p></summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <seealso cref="VisitSelector" />
    public override SelectorQueryResult VisitRecordSelectorCreation(AmethystParser.RecordSelectorCreationContext context)
    {
        var recordSelectors = new List<string>();

        var recordSelectorElementContexts = context.recordSelectorElement();
        ProcessRecordSelectorElements(recordSelectorElementContexts, ref recordSelectors, out var containsRuntimeValues);

        return new SelectorQueryResult("records", $"scores={{{string.Join(",", recordSelectors)}}}", containsRuntimeValues);
    }

    private void ProcessRecordSelectorElements(IEnumerable<AmethystParser.RecordSelectorElementContext> recordSelectorElementContexts, ref List<string> recordSelectors, out bool containsRuntimeValues)
    {
        containsRuntimeValues = false;
        
        foreach (var recordSelectorElementContext in recordSelectorElementContexts)
        {
            if (recordSelectorElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                var result = VisitPreprocessorYieldingStatement<AmethystParser.RecordSelectorElementContext>(preprocessorYieldingStatementContext);
                ProcessRecordSelectorElements(result, ref recordSelectors, out containsRuntimeValues);
            }
            else if (recordSelectorElementContext.recordSelectorKvp() is { } recordSelectorKvpContext)
            {
                var queryResult = VisitRecordSelectorKvp(recordSelectorKvpContext);
                containsRuntimeValues |= queryResult.ContainsRuntimeValues;
                recordSelectors.Add(queryResult.ToString());
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
        
        // BUG: This does not handle the record's datatypes correctly.
        //  (decimals should be allowed; complex datatypes are not handled at all currently)
        // TODO: Think about how to handle this (maybe only consider numeric records?)

        // if (context.expression() is { } expressionContext)
        // {
        //     queryResult = VisitNumericSelector(record.Name, expressionContext, allowDecimals: false);
        // }
        // else if (context.rangeExpression() is { } rangeExpressionContext)
        // {
        //     queryResult = VisitRangeSelector(record.Name, rangeExpressionContext, allowDecimals: false);
        // }
        // else
        // {
            throw new SyntaxException("Expected numeric or range expression.", context);
        // }

        return queryResult;
    }
}