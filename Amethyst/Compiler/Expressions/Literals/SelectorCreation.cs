using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>Creates a target selector. The supplied query parameters are sorted into an optimal order
    ///     and the resulting datatype is determined based on whether the selector can select multiple
    ///     targets or not to ensure optimal type safety.</p>
    ///     <p><inheritdoc /></p></summary>
    public override AbstractValue VisitSelectorCreation(AmethystParser.SelectorCreationContext context)
    {
        var selectorTypeContext = context.selectorType();
        var selector = VisitSelectorType(selectorTypeContext);

        var sp = StackPointer;

        var selectorElementContexts = context.selectorElement();
        var queryList = new Dictionary<string, string>(); 
        ProcessSelectorElements(selectorElementContexts, ref queryList, out var containsRuntimeValues, out var limitExpression);
        
        var selectorString = $"{selector.GetMcfOperatorSymbol()}{QueryListToString(queryList)}";

        StackPointer = sp;

        if (containsRuntimeValues)
        {
            AbstractValue value;

            string mcFunctionPath;
            using (this.EvaluateScoped("_create_selector"))
            {
                mcFunctionPath = Scope.McFunctionPath;
                value = CreateSelector(selector, limitExpression, selectorString, context, isMacroInvocation: true);
            }
            
            this.AddCode($"function {mcFunctionPath} with storage amethyst:");

            return value;
        }

        return CreateSelector(selector, limitExpression, selectorString, context);
    }

    /// <summary><p>Returns the target selector enum from the selector type context.</p>
    ///     <p><inheritdoc cref="AmethystParserBaseVisitor{Result}.VisitSelectorType" /></p></summary>
    /// <example><c>@s</c> -> <see cref="TargetSelector.Self" /><br /> <c>@e</c> ->
    /// <see cref="TargetSelector.AllEntities" /></example>
    public new static TargetSelector VisitSelectorType(AmethystParser.SelectorTypeContext context)
    {
        var selector = context.GetText();
        return EnumExtension.GetEnumFromMcfOperator<TargetSelector>(selector);
    }

    /// <summary>
    ///     <p>Processes the selector query elements and determines if any of them are dynamic. Optionally
    ///     returns a limit expression if one is found, influencing the final datatype of this target
    ///     selector.</p>
    /// </summary>
    /// <param name="selectorElementContexts">The selector element contexts to process.</param>
    /// <param name="queryList">The query list to populate.</param>
    /// <param name="containsRuntimeValues">Whether any of the selector elements contain runtime values.</param>
    /// <param name="limitExpression">The limit expression, if found.</param>
    /// <exception cref="SyntaxException">Thrown if a duplicate selector key is found.</exception>
    private void ProcessSelectorElements(IEnumerable<AmethystParser.SelectorElementContext> selectorElementContexts, ref Dictionary<string, string> queryList, out bool containsRuntimeValues, out AbstractValue? limitExpression)
    {
        containsRuntimeValues = false;
        limitExpression = null;
        
        foreach (var selectorElementContext in selectorElementContexts)
        {
            if (selectorElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                var result = VisitPreprocessorYieldingStatement<AmethystParser.SelectorElementContext>(preprocessorYieldingStatementContext);
                ProcessSelectorElements(result, ref queryList, out containsRuntimeValues, out limitExpression);
            }
            else if (selectorElementContext.selectorKvp() is { } selectorKvpContext)
            {
                var queryResult = VisitSelector(selectorKvpContext);

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

    /// <summary>
    ///     <p>Creates the target selector value based on whether it is single-target or multi-target.</p>
    /// </summary>
    /// <param name="selector">The base target selector.</param>
    /// <param name="limitExpression">The limit expression, if any.</param>
    /// <param name="selectorString">The target selector string ready to be used in Minecraft.</param>
    /// <param name="context">The selector creation compiler context.</param>
    /// <param name="isMacroInvocation">Whether this selector is being created as part of a macro
    /// invocation. This is the case when any of the selector elements are dynamic and need to be supplied
    /// at runtime.</param>
    /// <returns>>The created target selector value.</returns>
    private AbstractValue CreateSelector(TargetSelector selector, AbstractValue? limitExpression, string? selectorString, AmethystParser.SelectorCreationContext context, bool isMacroInvocation = false)
    {
        var prefix = isMacroInvocation ? "$" : string.Empty;
            
        var location = Location.Storage(++StackPointer);

        if (IsSelectorSingleTarget(selector, limitExpression))
        {
            this.AddCode($"{prefix}execute as {selectorString} run function amethyst:libraries/gu/generate");
            this.AddCode($"data modify storage {location} set from storage gu:main out");
            
            return new RuntimeEntity
            {
                Compiler = this,
                Context = context,
                Location = location,
                IsTemporary = true
            };
        }
        
        this.AddCode($"data modify storage {location} set value []");

        string mcFunctionPath;
        using (this.EvaluateScoped("_multi_selector"))
        {
            mcFunctionPath = Scope.McFunctionPath;
            this.AddCode("function amethyst:libraries/gu/generate");
            this.AddCode($"data modify storage {location} append from storage gu:main out");
        }

        this.AddCode($"{prefix}execute as {selectorString} run function {mcFunctionPath}");

        return new RuntimeStaticArray
        {
            Compiler = this,
            Context = context,
            BasicType = BasicType.Entity,
            Location = location,
            IsTemporary = true
        };
    }

    /// <summary>
    ///     <p>Determines whether the given target selector with the provided limit expression is a
    ///     single-target selector. The exact rules for this evaluation is taken from
    ///     https://minecraft.wiki/w/Target_selectors#Single_type which state that
    ///     <i>A single-type selector is a selector that can only select one target, including:</i>
    ///     <list type="bullet"><item>@a, @e with limit=1</item> <item>@s without limit argument</item>
    ///         <item>@p, @r without limit argument or with limit=1</item></list>
    ///     </p>
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="selectorLimit"></param>
    /// <returns></returns>
    private static bool IsSelectorSingleTarget(TargetSelector selector, AbstractValue? selectorLimit)
    {
        // if the selector limit is not known at compile time, assume it's a multi-selector
        if (selectorLimit is IRuntimeValue)
        {
            return false;
        }
        
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

    /// <summary>
    ///     <p>Converts the query list dictionary into a properly formatted selector query string, ordering
    ///     the keys in an optimal order for Minecraft to process.</p>
    /// </summary>
    /// <param name="queryList">The query list dictionary.</param>
    /// <returns>The formatted selector query string.</returns>
    private static string QueryListToString(IReadOnlyDictionary<string, string> queryList)
    {
        if (queryList.Count == 0)
        {
            return "";
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