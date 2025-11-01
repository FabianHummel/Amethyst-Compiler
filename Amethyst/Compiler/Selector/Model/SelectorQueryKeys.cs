namespace Amethyst.Model;

public static partial class Constants
{
    // source: https://minecraft.wiki/w/Target_selectors#Target_selector_arguments
    public static readonly IReadOnlyDictionary<string, AbstractQuerySelector> TargetSelectorQueryKeys = new Dictionary<string, AbstractQuerySelector>()
    {
        { "x", new RangeSelector(allowDecimals: true) },
        { "y", new RangeSelector(allowDecimals: true) },
        { "z", new RangeSelector(allowDecimals: true) },
        { "distance", new DistanceSelector() },
        { "dx", new RangeSelector(allowDecimals: true) },
        { "dy", new RangeSelector(allowDecimals: true) },
        { "dz", new RangeSelector(allowDecimals: true) },
        { "x_rotation", new RangeSelector(allowDecimals: true, minValue: -90.0, maxValue: 90.0) },
        { "y_rotation", new RangeSelector(allowDecimals: true, minValue: -180.0, maxValue: 180.0) },
        { "records", new RecordsSelector() },
        { "tag", new BasicSelector(allowMultipleEqualityChecks: true, BasicType.String) },
        { "tags", new MultiSelector("tag") },
        { "team", new BasicSelector(allowMultipleEqualityChecks: false, BasicType.String) },
        { "teams", new MultiSelector("team") },
        { "name", new BasicSelector(allowMultipleEqualityChecks: false, BasicType.String) },
        { "names", new MultiSelector("name") },
        { "type", new BasicSelector(allowMultipleEqualityChecks: false, BasicType.String) },
        { "types", new MultiSelector("type") },
        { "predicate", new BasicSelector(allowMultipleEqualityChecks: true, BasicType.String) },
        { "predicates", new MultiSelector("predicate") },
        { "nbt", new BasicSelector(allowMultipleEqualityChecks: true, BasicType.Object) },
        { "gamemode", new LiteralSelector(["spectator", "survival", "creative", "adventure"]) },
        { "gamemodes", new MultiSelector("gamemode") },
        { "advancements", new AdvancementsSelector() },
        { "limit", new LimitSelector() },
        { "sort", new LiteralSelector(["nearest", "furthest", "random", "arbitrary"]) }
    };
}