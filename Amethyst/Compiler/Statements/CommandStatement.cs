using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Parses a raw MCF command. The command is embedded as-is into the resulting datapack code.
    ///     This is useful for interacting with native libraries or calling functions that are not
    ///     supported by Amethyst.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><c>/time set day</c></example>
    public override object? VisitCommandStatement(AmethystParser.CommandStatementContext context)
    {
        this.AddCode(context.COMMAND().GetText().TrimStart()[1..]);
        return null;
    }
}