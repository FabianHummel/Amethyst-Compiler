using Amethyst.Model;
using Amethyst.Utility;
using OutParsing;
using RconSharp;

namespace Tests;

public class McfTestContext
{
    private readonly RconClient rcon;
    private readonly Scope scope;

    public McfTestContext(RconClient rcon, Scope scope)
    {
        this.rcon = rcon;
        this.scope = scope;
    }
    
    public async Task<T> Variable<T>(string identifier)
    {
        var symbol = GetSymbolOrThrow(identifier);

        if (symbol is not Variable variable)
        {
            throw new UnitTestException($"Symbol is not a variable: {identifier}");
        }

        if (variable.Location.DataLocation == DataLocation.Scoreboard)
        {
            var scoreboardValue = await GetScoreboardValue(variable.Location);
            return (T)NbtUtility.ParseScoreboardValue(scoreboardValue, variable.Datatype);
        }

        if (variable.Location.DataLocation == DataLocation.Storage)
        {
            var storageValue = await GetStorageValue(variable.Location);
            return (T)NbtUtility.ParseStorageValue(storageValue, variable.Datatype);
        }

        throw new InvalidOperationException($"Unknown data location '{variable.Location.DataLocation}'.");
    }
    
    private Symbol GetSymbolOrThrow(string identifier)
    {
        if (!scope.TryGetSymbol(identifier, out var symbol))
        {
            throw new UnitTestException($"Symbol not found: {identifier}");
        }

        return symbol;
    }

    private async Task<int> GetScoreboardValue(Location location)
    {
        var command = $"scoreboard players get {location}";
        var result = await rcon.ExecuteCommandAsync(command);
        if (result == null)
        {
            throw new UnitTestException($"'{command}' failed to execute.");
        }
        
        var successfulResult = OutParser.TryParse(result, 
            "{variableName} has {value} [{scoreboardName}]", // TODO: Add versioning for different output formats
            out int value, out string variableName, out string scoreboardName);
        
        var failedResult = OutParser.TryParse(result, 
            "Can't get value of {scoreboardName} for {variableName}; none is set", 
            out scoreboardName, out variableName);
        
        if (!successfulResult && !failedResult)
        {
            throw new UnitTestException($"Failed to parse result: {result}");
        }
        
        if (failedResult)
        {
            throw new UnitTestException($"Scoreboard value not set: {variableName} in {scoreboardName}");
        }
        
        return value;
    }

    private async Task<string> GetStorageValue(Location location)
    {
        var command = $"data get storage {location}";
        var result = await rcon.ExecuteCommandAsync(command);
        if (result == null)
        {
            throw new UnitTestException($"'{command}' failed to execute.");
        }
        
        var successfulResult = OutParser.TryParse(result, 
            "Storage {_} has the following contents: {value}", // TODO: Add versioning for different output formats
            out string _, out string value);
        
        var failedResult = OutParser.TryParse(result,
            "Found no elements matching {storageLocation}",
            out string storageLocation);

        if (!successfulResult && !failedResult)
        {
            throw new UnitTestException($"Failed to parse result: {result}");
        }
        
        if (failedResult)
        {
            throw new UnitTestException($"Storage value not set: {storageLocation}");
        }
        
        return value;
    }

    public async Task WaitForCompletion()
    {
        const string command = "execute if score #completed amethyst_test matches 1";
        
        while (true)
        {
            var result = await rcon.ExecuteCommandAsync(command);
            if (!OutParser.TryParse(result, "Test {status}", out string status))
            {
                throw new UnitTestException($"Failed to retrieve test completion status: {result}");
            }

            if (status == "passed")
            {
                break;
            }
        }
    }
}