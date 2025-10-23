namespace Amethyst.Model;

public class Variable : Symbol
{
    public required string Name { get; init; }
    public required Location Location { get; init; }
    public required AbstractDatatype Datatype { get; init; }
    public required HashSet<string> Attributes { get; init; }

    public string ToMcfValue(object value)
    {
        if (Location.DataLocation == DataLocation.Scoreboard)
        {
            if (Datatype is DecimalDatatype decimalDatatype)
            {
                value = Math.Round((double)value, decimalDatatype.DecimalPlaces);
            }
            
            if (!IScoreboardValue.TryParse(value, out var scoreboardValue))
            {
                throw new InvalidOperationException($"Cannot convert value '{value}' to scoreboard value.");
            }
            return scoreboardValue.ScoreboardValue.ToString();
        }

        if (Location.DataLocation == DataLocation.Storage)
        {
            if (!IConstantValue.TryParse(value, out var storageValue))
            {
                throw new InvalidOperationException($"Cannot convert value '{value}' to storage value.");
            }
            return storageValue.ToNbtString();
        }

        throw new InvalidOperationException($"Unknown data location '{Location.DataLocation}'.");
    }

    public override string ToString()
    {
        var attributes = Attributes.Count > 0 ? $"[{string.Join(", ", Attributes)}] " : "";
        return $"{attributes}{Datatype} ({Location})";
    }
}