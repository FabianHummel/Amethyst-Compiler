namespace Tests;

public class MinecraftServerSettings
{
    public required string JarPath { get; init; }
    public required string RconHost { get; init; }
    public required int RconPort { get; init; }
    public required string RconPassword { get; init; }
}
