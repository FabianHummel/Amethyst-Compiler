using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public RawDatatype VisitRawLocation(AmethystParser.RawLocationContext rawLocationContext)
    {
        return (RawDatatype)Visit(rawLocationContext)!;
    }
    
    public override RawDatatype VisitRawScoreboardLocation(AmethystParser.RawScoreboardLocationContext context)
    {
        return new RawDatatype(DataLocation.Scoreboard)
        {
            Namespace = context.SCOREBOARD_OBJECTIVE().GetText(),
            Name = context.SCOREBOARD_PLAYER().GetText()
        };
    }
    
    public override RawDatatype VisitRawStorageLocation(AmethystParser.RawStorageLocationContext context)
    {
        return new RawDatatype(DataLocation.Storage)
        {
            Namespace = context.STORAGE_NAMESPACE().GetText(),
            Name = context.STORAGE_MEMBER().GetText()
        };
    }
}