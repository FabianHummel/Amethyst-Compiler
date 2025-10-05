using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitSelectorCreation(AmethystParser.SelectorCreationContext context)
    {
        var selectorTypeContext = context.selectorType();
        var selector = VisitSelectorType(selectorTypeContext);
        
        var queryList = new Dictionary<string, string>();
        var containsRuntimeValues = false;
        AbstractValue? limitExpression = null;

        var sp = StackPointer;

        var selectorElementContexts = context.selectorElement();
        ProcessSelectorElements(selectorElementContexts);
        
        var selectorString = $"{selector.GetMcfOperatorSymbol()}{QueryListToString(queryList)}";

        StackPointer = sp;

        if (containsRuntimeValues)
        {
            AbstractValue value = null!;
            
            var scope = EvaluateScoped("_create_selector", _ =>
            {
                value = CreateSelector(isMacroInvocation: true);
            });
            
            AddCode($"function {scope.McFunctionPath} with storage amethyst:");

            return value;
        }

        return CreateSelector();

        void ProcessSelectorElements(IEnumerable<AmethystParser.SelectorElementContext> selectorElementContexts)
        {
            foreach (var selectorElementContext in selectorElementContexts)
            {
                if (selectorElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
                {
                    var result = VisitPreprocessorYieldingStatement<AmethystParser.SelectorElementContext>(preprocessorYieldingStatementContext);
                    ProcessSelectorElements(result);
                }
                else if (selectorElementContext.selectorKvp() is { } selectorKvpContext)
                {
                    var queryResult = VisitSelectorKvp(selectorKvpContext);

                    if (queryList.ContainsKey(queryResult.QueryKey))
                    {
                        throw new SyntaxException($"Duplicate selector '{queryResult.QueryKey}'.", selectorKvpContext);
                    }
                
                    containsRuntimeValues |= queryResult.ContainsRuntimeValues;
                    limitExpression ??= queryResult.LimitExpression;
                    queryList.Add(queryResult.QueryKey, queryResult.QueryString);
                }
            }
        }

        AbstractValue CreateSelector(bool isMacroInvocation = false)
        {
            var prefix = isMacroInvocation ? "$" : string.Empty;
            
            var location = ++StackPointer;

            if (IsSelectorSingleTarget(selector, limitExpression))
            {
                AddCode($"{prefix}execute as {selectorString} run function amethyst:libraries/gu/generate");
                AddCode($"data modify storage amethyst: {location} set from storage gu:main out");
            
                return new RuntimeEntity
                {
                    Compiler = this,
                    Context = context,
                    Location = location,
                    IsTemporary = true
                };
            }
        
            AddCode($"data modify storage amethyst: {location} set value []");

            var scope = EvaluateScoped("_multi_selector", _ =>
            {
                AddCode("function amethyst:libraries/gu/generate");
                AddCode($"data modify storage amethyst: {location} append from storage gu:main out");
            });
        
            AddCode($"{prefix}execute as {selectorString} run function {scope.McFunctionPath}");

            return new RuntimeStaticArray
            {
                Compiler = this,
                Context = context,
                BasicType = BasicType.Entity,
                Location = location,
                IsTemporary = true
            };
        }
    }
    
    public override SelectorQueryResult VisitSelectorKvp(AmethystParser.SelectorKvpContext context)
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
                return VisitTagsSelector(expressionContext);
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

        if (context is AmethystParser.RecordSelectorContext recordSelectorContext)
        {
            var identifier = recordSelectorContext.IDENTIFIER();
            var queryKey = identifier.GetText();

            if (queryKey is "records")
            {
                var recordSelectorCreationContext = recordSelectorContext.recordSelectorCreation();
                return VisitRecordSelectorCreation(recordSelectorCreationContext);
            }
        }

        throw new InvalidOperationException($"Invalid selector '{context}'.");
    }

    public new static TargetSelector VisitSelectorType(AmethystParser.SelectorTypeContext context)
    {
        var selector = context.GetText();
        return EnumExtension.GetEnumFromMcfOperator<TargetSelector>(selector);
    }
    
    private static bool IsSelectorSingleTarget(TargetSelector selector, AbstractValue? selectorLimit)
    {
        // if the selector limit is not known at compile time, assume it's a multi-selector
        if (selectorLimit is IRuntimeValue)
        {
            return false;
        }
        
        /* Source: https://minecraft.wiki/w/Target_selectors#Single_type
         A single-type selector is a selector that can only select one target, including:
         · @a, @e with limit=1.
         · @s without limit argument.
         · @p, @r without limit argument or with limit=1
         */
        
        if (selector is TargetSelector.AllPlayers or TargetSelector.AllEntities && selectorLimit is ConstantInteger { Value: 1 })
        {
            return true;
        }
        
        if (selector is TargetSelector.Self && selectorLimit is null)
        {
            return true;
        }
        
        if (selector is TargetSelector.NearestPlayer or TargetSelector.NearestEntity or TargetSelector.RandomPlayer && selectorLimit is null or ConstantInteger { Value: 1 })
        {
            return true;
        }

        return false;
    }
    
    private static string? QueryListToString(IReadOnlyDictionary<string, string> queryList)
    {
        if (queryList.Count == 0)
        {
            return null;
        }

        var optimalQueryOrder = new[]
        {
            "x", "y", "z", "dx", "dy", "dz",
            "type", "level", "gamemode", "team",
            "tag", "name", "records", "distance",
            "x_rotation", "y_rotation", "advancement",
            "predicate", "nbt", "limit", "sort"
        };
        
        var queryString = new List<string>();

        foreach (var queryKey in optimalQueryOrder)
        {
            if (queryList.TryGetValue(queryKey, out var queryValue))
            {
                queryString.Add(queryValue);
            }
        }

        return $"[{string.Join(",", queryString)}]";
    }
}