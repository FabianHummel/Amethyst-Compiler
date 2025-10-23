using Amethyst.Model;
using Amethyst.Utility;
using NUnit.Framework;
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

    public Task<T> Variable<T>(string identifier)
    {
        return Variable<T>(identifier, out _);
    }
    
    public Task<T> Variable<T>(string identifier, out Variable variable)
    {
        var symbol = GetSymbolOrThrow(identifier);

        if (symbol is not Variable variable2)
        {
            throw new UnitTestException($"Symbol is not a variable: {identifier}");
        }
        
        variable = variable2;

        if (variable2.Location.DataLocation == DataLocation.Scoreboard)
        {
            return GetScoreboardValue(variable2.Location).ContinueWith(t =>
            {
                return (T)NbtUtility.ParseScoreboardValue(t.Result, variable2.Datatype);
            });
        }

        if (variable2.Location.DataLocation == DataLocation.Storage)
        {
            return GetStorageValue(variable2.Location).ContinueWith(t =>
            {
                return (T)NbtUtility.ParseStorageValue(t.Result, variable2.Datatype);
            });
        }

        throw new InvalidOperationException($"Unknown data location '{variable2.Location.DataLocation}'.");
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
        TestContext.WriteLine($"/{command}");
        var result = await rcon.ExecuteCommandAsync(command);
        if (result == null)
        {
            throw new UnitTestException($"'{command}' failed to execute.");
        }
        
        TestContext.WriteLine($"-> {result}");
        
        var successfulResult = OutParser.TryParse(result, 
            "{_1} has {value} [{_2}]", // TODO: Add versioning for different output formats
            out int value, out string _1, out string _2);
        
        bool failedResult = false;
        if (!successfulResult)
        {
            failedResult = OutParser.TryParse(result,
                "Can't get value of {scoreboardName} for {variableName}; none is set",
                out string scoreboardName, out string variableName);
            if (failedResult)
            {
                throw new UnitTestException($"Scoreboard value not set: {variableName} in {scoreboardName}");
            }
        }
        
        if (!successfulResult && !failedResult)
        {
            throw new UnitTestException($"Failed to parse result: {result}");
        }
        
        return value;
    }

    private async Task<string> GetStorageValue(Location location)
    {
        var command = $"data get storage {location}";
        TestContext.WriteLine($"/{command}");
        var result = await rcon.ExecuteCommandAsync(command);
        if (result == null)
        {
            throw new UnitTestException($"'{command}' failed to execute.");
        }
        
        var successfulResult = OutParser.TryParse(result, 
            "Storage {_} has the following contents: {value}", // TODO: Add versioning for different output formats
            out string _, out string value);
        
        var failedResult = false;
        if (!successfulResult)
        {
            failedResult = OutParser.TryParse(result,
                "Found no elements matching {storageLocation}",
                out string storageLocation);
            if (failedResult)
            {
                throw new UnitTestException($"Storage value not set: {storageLocation}");
            }
        }

        if (!successfulResult && !failedResult)
        {
            throw new UnitTestException($"Failed to parse result: {result}");
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