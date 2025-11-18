namespace Amethyst.Model;

/// <summary>Represents a Minecraft entity target. Combined with a selector, it results in a full
/// target selector.</summary>
/// <seealso cref="Compiler.VisitSelectorCreation" />
public enum EntityTarget
{
    [McfToken("@s")]
    Self,
    [McfToken("@r")]
    RandomPlayer,
    [McfToken("@a")]
    AllPlayers,
    [McfToken("@e")]
    AllEntities,
    [McfToken("@p")]
    NearestPlayer,
    [McfToken("@n")]
    NearestEntity
}

/// <summary>Wrapper class for the <see cref="EntityTarget" /> enum so it can be returned by a visitor
/// method.</summary>
public class EntityTargetResult
{
    /// <summary>The contained entity target.</summary>
    public EntityTarget Target { get; }

    /// <summary>Initializes a new instance of <see cref="EntityTargetResult" />.</summary>
    /// <param name="target">The contained entity target.</param>
    public EntityTargetResult(EntityTarget target)
    {
        Target = target;
    }

    /// <summary>Determines whether an entity is considered a valid "single-target" based on
    /// https://minecraft.wiki/w/Target_selectors#Single_type. A single-target is any selector that is
    /// guaranteed to match only a single entity and unlocks special benefits. This includes:
    /// <list type="bullet"><item><c>@a</c>, <c>@e</c> with <c>limit=1</c>.</item>
    ///     <item><c>@s</c> without <c>limit</c> argument.</item>
    ///     <item><c>@p</c>, <c>@r</c> without <c>limit</c> argument or with <c>limit=1</c></item></list>
    /// </summary>
    /// <param name="selectorLimit">The <c>limit=</c> query value if specified in the selector query. This
    /// is an essential part of determining if a selector can only match a single entity.</param>
    /// <returns>True, if the selector is guaranteed to match only a single entity, false otherwise.</returns>
    /// <example><p><c>@e[limit=1]</c> → true</p> <p><c>@p</c> → true</p>
    ///     <p><c>@a[tag="test"]</c> → false</p></example>
    public bool IsSingleTarget(AbstractValue? selectorLimit)
    {
        // if the selector limit is not known at compile time, assume it's a multi-selector
        if (selectorLimit is IRuntimeValue)
        {
            return false;
        }
        
        if (Target is EntityTarget.AllPlayers or EntityTarget.AllEntities && selectorLimit is ConstantInteger { Value: 1 })
        {
            return true;
        }
        
        if (Target is EntityTarget.Self && selectorLimit is null) // TODO: Make "@s[limit=...]" a syntax error!
        {
            return true;
        }
        
        if (Target is EntityTarget.NearestPlayer or EntityTarget.NearestEntity or EntityTarget.RandomPlayer && selectorLimit is null or ConstantInteger { Value: 1 })
        {
            return true;
        }

        return false;
    }
}