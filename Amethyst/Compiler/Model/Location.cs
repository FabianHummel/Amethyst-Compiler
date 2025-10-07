namespace Amethyst.Model;

public class Location
{
    public string Namespace { get; init; }
    public required string Name { get; init; }
    public required DataLocation DataLocation { get; init; }

    public Location(string? ns = null)
    {
        Namespace = ns ?? "amethyst";
    }

    public override string ToString()
    {
        if (DataLocation == DataLocation.Scoreboard)
        {
            return $"{Name} {Namespace}";
        }

        string storageNs;
        if (Namespace.Contains(':'))
        {
            storageNs = Namespace;
        }
        else
        {
            storageNs = $"{Namespace}:";
        }
        
        return $"{storageNs} {Name}";
    }

    public static implicit operator string(Location location) => location.ToString();
    
    public static Location Storage(int location, string? ns = null)
    {
        return Storage(location.ToString(), ns);
    }

    public static Location Storage(string location, string? ns = null)
    {
        return new Location(ns)
        {
            DataLocation = DataLocation.Storage,
            Name = location
        };
    }
    
    public static Location Scoreboard(int location, string? ns = null)
    {
        return Scoreboard(location.ToString(), ns);
    }
    
    public static Location Scoreboard(string location, string? ns = null)
    {
        return new Location(ns)
        {
            DataLocation = DataLocation.Scoreboard,
            Name = location
        };
    }
}