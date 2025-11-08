namespace Amethyst.Model;

/// <summary>Represents a function definition within the amethyst source code.</summary>
public class Function : Symbol
{
    /// <summary>A set of attributes linked to the function. For example, functions with an
    /// <see cref="Constants.AttributeLoadFunction" /> are run once automatically on startup.</summary>
    public required HashSet<string> Attributes { get; init; }

    /// <summary>List of parameters of the function. I chose to use an array of variables, because that's
    /// what function parameters actually are. This makes it very intuitive and easy to use.</summary>
    public required Variable[] Parameters { get; init; }

    /// <summary>The MCFunction path that is used to call the function in the datapack code.</summary>
    public required string McFunctionPath { get; init; }
}