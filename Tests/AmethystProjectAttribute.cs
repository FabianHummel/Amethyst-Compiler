namespace Tests;

[AttributeUsage(AttributeTargets.Method)]
public class AmethystProjectAttribute : Attribute
{
    public required string Path { get; set; }
}