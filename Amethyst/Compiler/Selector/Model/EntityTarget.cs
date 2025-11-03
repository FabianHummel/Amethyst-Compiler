namespace Amethyst.Model;

public enum EntityTarget
{
    [McfOperator("@s")]
    Self,
    [McfOperator("@r")]
    RandomPlayer,
    [McfOperator("@a")]
    AllPlayers,
    [McfOperator("@e")]
    AllEntities,
    [McfOperator("@p")]
    NearestPlayer,
    [McfOperator("@n")]
    NearestEntity
}

public class EntityTargetResult
{
    public EntityTarget Target { get; }
    
    public EntityTargetResult(EntityTarget target)
    {
        Target = target;
    }
    
    public bool IsSingleTarget(AbstractValue? selectorLimit)
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
        
        if (Target is EntityTarget.AllPlayers or EntityTarget.AllEntities && selectorLimit is ConstantInteger { Value: 1 })
        {
            return true;
        }
        
        if (Target is EntityTarget.Self && selectorLimit is null)
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