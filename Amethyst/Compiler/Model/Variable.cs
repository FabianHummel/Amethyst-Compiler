namespace Amethyst.Model;

public class Variable : Symbol
{
    public required int Location { get; init; }
    public required DataType DataType { get; init; }
    public required HashSet<string> Attributes { get; init; }

    public string ToMcfValue(object value)
    {
        if (DataType.Location == DataLocation.Scoreboard)
        {
            if (!IScoreboardValue.TryParse(value, out var scoreboardValue))
            {
                throw new InvalidOperationException($"Cannot convert value '{value}' to scoreboard value.");
            }
            return scoreboardValue.ScoreboardValue.ToString();
        }

        if (DataType.Location == DataLocation.Storage)
        {
            if (!IConstantValue.TryParse(value, out var storageValue))
            {
                throw new InvalidOperationException($"Cannot convert value '{value}' to storage value.");
            }
            return storageValue.ToNbtString();
        }

        throw new InvalidOperationException($"Unknown data location '{DataType.Location}'.");
    }

    public override string ToString()
    {
        return $"{Location}: {DataType} [{string.Join(',', Attributes)}]";
    }
}