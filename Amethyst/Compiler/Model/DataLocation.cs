namespace Amethyst.Model;

/// <summary>Denotes the internal location-type of a variable in amethyst. Fundamentally, all numeric
/// values are stored in scoreboards as this enables arithmetic operations, while complex types such as
/// arrays, objects or entities are stored in storages.</summary>
public enum DataLocation
{
    Scoreboard,
    Storage
}