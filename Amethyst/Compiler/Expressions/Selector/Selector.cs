using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>Visits a selector key-value pair context and returns the corresponding query result. The
    ///     query selector syntax is a little different from traditional Minecraft's syntax to better
    ///     integrate Amethyst's language features.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="InvalidOperationException">The selector key-value pair is invalid.</exception>
    public SelectorQueryResult VisitSelector(AmethystParser.SelectorKvpContext context)
    {
        return (SelectorQueryResult)Visit(context)!;
    }
}