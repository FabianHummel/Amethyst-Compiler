namespace Amethyst.Model;

public abstract class Symbol
{
    public required HashSet<string> Attributes { get; init; }
}