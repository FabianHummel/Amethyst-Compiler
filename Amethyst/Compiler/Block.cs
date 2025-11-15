using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Parses a block of statements. This creates a new function under the <c>_block</c> directory
    ///     and calls it.</p>
    ///     <p><inheritdoc /></p></summary>
    public override object? VisitBlock(AmethystParser.BlockContext context)
    {
        var mcFunctionPath = VisitBlockNamed(context, "_block");
        this.AddCode($"function {mcFunctionPath}");
        return null;
    }

    /// <summary>Parses a block of statements. Unlike <see cref="VisitBlock" /> this method does not create
    /// a separate function, but evaluates the contents in the current scope.</summary>
    /// <param name="context">The parse tree.</param>
    public void VisitBlockInline(AmethystParser.BlockContext context)
    {
        foreach (var preprocessorStatementContext in context.preprocessorStatement())
        {
            Visit(preprocessorStatementContext);
        }
    }

    /// <summary>Parses a block of statements. This creates a new function under a directory with the
    /// specified <paramref name="name" /> and calls it.</summary>
    /// <param name="context">The parse tree.</param>
    /// <param name="name">The name of the enclosing directory where the function will be put under.</param>
    /// <param name="preserveName">Whether to keep the name as-is. If false, an incrementing number is
    /// appended to the name to prevent duplicates.</param>
    /// <returns>The path to the created function in a format that is used to call it in the datapack code.</returns>
    internal string VisitBlockNamed(AmethystParser.BlockContext context, string name, bool preserveName = false)
    {
        using var scope = this.EvaluateScoped(name, preserveName);
        VisitBlockInline(context);
        return Scope.McFunctionPath;
    }
}