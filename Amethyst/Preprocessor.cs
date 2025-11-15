using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>The current yielding scope that is used in yield-loops to collect the aggregated result.</summary>
    public YieldingScope YieldingScope { get; set; } = null!;
    
    /// <summary>The current looping scope that is used to break out of the current loop.</summary>
    public LoopingScope LoopingScope { get; set; } = null!;
}