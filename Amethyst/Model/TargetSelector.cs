using Amethyst.Model.Attributes;

namespace Amethyst.Model;

public enum TargetSelector
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