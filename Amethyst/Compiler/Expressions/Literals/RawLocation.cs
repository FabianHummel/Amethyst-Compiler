using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary><p>Helper method to visit a raw location context.</p></summary>
    /// <seealso cref="VisitRawScoreboardLocation" />
    /// <seealso cref="VisitRawStorageLocation" />
    public RawDatatype VisitRawLocation(AmethystParser.RawLocationContext rawLocationContext)
    {
        return (RawDatatype)Visit(rawLocationContext)!;
    }

    /// <inheritdoc />
    /// <summary>
    ///     <p>A raw scoreboard location in the format <c>&lt;location&gt; &lt;namespace&gt;</c> or in
    ///     Minecraft literals <c>&lt;player&gt; &lt;objective&gt;</c>.</p>
    ///     <p><inheritdoc /></p></summary>
    public override RawDatatype VisitRawScoreboardLocation(AmethystParser.RawScoreboardLocationContext context)
    {
        return new RawDatatype(DataLocation.Scoreboard)
        {
            Namespace = context.SCOREBOARD_OBJECTIVE().GetText(),
            Name = context.SCOREBOARD_PLAYER().GetText()
        };
    }

    /// <inheritdoc />
    /// <summary>
    ///     <p>A raw storage location in the format <c>&lt;namespace&gt; &lt;path.to.member&gt;</c>.</p>
    ///     <p><inheritdoc /></p></summary>
    public override RawDatatype VisitRawStorageLocation(AmethystParser.RawStorageLocationContext context)
    {
        return new RawDatatype(DataLocation.Storage)
        {
            Namespace = context.STORAGE_NAMESPACE().GetText(),
            Name = context.STORAGE_MEMBER().GetText()
        };
    }
}