using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitSelector_expression(AmethystParser.Selector_expressionContext context)
    {
        if (context.selector_type() is not { } selectorTypeContext)
        {
            throw new SyntaxException("Expected selector type.", context);
        }

        var selector = VisitSelector_type(selectorTypeContext);
        
        var queryList = new Dictionary<string, string>();
        var containsRuntimeValues = false;
        AbstractResult? limitExpression = null;

        var sp = StackPointer;
        
        if (context.selector_query_list() is { } selectorQueryListContext)
        {
            if (selectorQueryListContext.selector_query() is not { } selectorQueryList)
            {
                throw new SyntaxException("Expected selector query.", context);
            }
            
            foreach (var selectorQueryContext in selectorQueryList)
            {
                var selectorQuery = VisitSelector_query(selectorQueryContext);

                if (queryList.ContainsKey(selectorQuery.QueryKey))
                {
                    throw new SyntaxException($"Duplicate selector '{selectorQuery.QueryKey}'.", selectorQueryContext);
                }
                
                containsRuntimeValues |= selectorQuery.ContainsRuntimeValues;
                limitExpression ??= selectorQuery.LimitExpression;
                queryList.Add(selectorQuery.QueryKey, selectorQuery.QueryString);
            }
        }
        
        var selectorString = $"{selector.GetMcfOperatorSymbol()}{QueryListToString(queryList)}";

        StackPointer = sp;

        if (containsRuntimeValues)
        {
            AbstractResult result = null!;
            
            var scope = EvaluateScoped("_create_selector", _ =>
            {
                result = CreateSelector(true);
            });
            
            AddCode($"function {scope.McFunctionPath} with storage amethyst:");

            return result;
        }

        return CreateSelector();

        AbstractResult CreateSelector(bool isMacroInvocation = false)
        {
            var prefix = isMacroInvocation ? "$" : string.Empty;
            
            var location = ++StackPointer;

            if (IsSelectorSingleTarget(selector, limitExpression))
            {
                AddCode($"{prefix}execute as {selectorString} run function gu:generate");
                AddCode($"data modify storage amethyst: {location} set from storage gu:main out");
            
                return new EntityResult
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
                AddCode("function gu:generate");
                AddCode($"data modify storage amethyst: {location} append from storage gu:main out");
            });
        
            AddCode($"{prefix}execute as {selectorString} run function {scope.McFunctionPath}");

            return new StaticArrayResult
            {
                Compiler = this,
                Context = context,
                BasicType = BasicType.Entity,
                Location = location,
                IsTemporary = true
            };
        }
    }

    public new TargetSelector VisitSelector_type(AmethystParser.Selector_typeContext context)
    {
        var selector = context.GetText();
        return EnumExtension.GetEnumFromMcfOperator<TargetSelector>(selector);
    }
    
    private static bool IsSelectorSingleTarget(TargetSelector selector, AbstractResult? selectorLimit)
    {
        // if the selector limit is not known at compile time, assume it's a multi-selector
        if (selectorLimit is RuntimeValue)
        {
            return false;
        }
        
        /* Source: https://minecraft.wiki/w/Target_selectors#Single_type
         A single-type selector is a selector that can only select one target, including:
         · @a, @e with limit=1.
         · @s without limit argument.
         · @p, @r without limit argument or with limit=1
         */
        
        if (selector is TargetSelector.AllPlayers or TargetSelector.AllEntities && selectorLimit is IntegerConstant { Value: 1 })
        {
            return true;
        }
        
        if (selector is TargetSelector.Self && selectorLimit is null)
        {
            return true;
        }
        
        if (selector is TargetSelector.NearestPlayer or TargetSelector.NearestEntity or TargetSelector.RandomPlayer && selectorLimit is null or IntegerConstant { Value: 1 })
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
            "predicate", "nbt", "limit", "sort",
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