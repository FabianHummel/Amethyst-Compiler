using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Exceptions;

namespace Tests;

public class ScoreboardObjectiveArgumentType : ArgumentType<Dictionary<string, int>>
{
    private static readonly SimpleCommandExceptionType OBJECTIVE_NOT_FOUND = new(new LiteralMessage("Objective not found"));

    private readonly Dictionary<string, Dictionary<string, int>> _scoreboard;

    public ScoreboardObjectiveArgumentType(Dictionary<string, Dictionary<string, int>> scoreboard)
    {
        _scoreboard = scoreboard;
    }

    public override Dictionary<string, int> Parse(IStringReader reader)
    {
        if (!_scoreboard.TryGetValue(reader.ReadString(), out var playerScores))
        {
            throw OBJECTIVE_NOT_FOUND.CreateWithContext(reader);
        }
        
        return playerScores;
    }
}