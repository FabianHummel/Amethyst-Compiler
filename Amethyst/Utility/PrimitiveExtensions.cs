namespace Amethyst.Utility;

public static class PrimitiveExtensions
{
    /// <summary>
    /// Converts a boolean to <c>if</c> when true and <c>unless</c> when false.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <returns></returns>
    public static string ToConditionalClause(this bool value) => value ? "if" : "unless";
}