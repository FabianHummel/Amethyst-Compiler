using Amethyst.Model;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public abstract class ServerTestBase
{
    protected McfTestContext Context { get; private set; } = null!;

    [SetUp]
    public async Task Setup()
    {
        var currentTest = TestContext.CurrentContext.Test;
        var methodName = currentTest.MethodName;

        var linkAttribute = (LinkAttribute?) GetType()
            .GetMethod(methodName!)!
            .GetCustomAttributes(typeof(LinkAttribute), false)
            .First();
        
        if (linkAttribute == null)
        {
            throw new InvalidOperationException($"Test method '{methodName}' must have a Link attribute when tested on a server.");
        }
        
        if (!TestMain.Amethyst.Context.UnitTests.TryGetValue(linkAttribute.Path, out var testFunctionScope))
        {
            throw new InvalidOperationException($"Test method '{methodName}' could not be linked to function in Amethyst code base.");
        }
        
        // setup function arguments by sending commands to the server
        var methodParameters = GetType()
            .GetMethod(methodName!)!
            .GetParameters();

        for (var index = 0; index < methodParameters.Length; index++)
        {
            var methodParameter = methodParameters[index];
            var parameterName = methodParameter.Name;
            if (!testFunctionScope.TryGetSymbol(parameterName!, out var parameterSymbol) || parameterSymbol is not Variable parameterVariable)
            {
                throw new InvalidOperationException($"Test function '{linkAttribute.Path}' does not have a parameter '{parameterName}'.");
            }
            
            var argumentValue = currentTest.Arguments[index];
            await SetVariableValue(parameterVariable, argumentValue!);
        }

        // run the test function on the server
        await TestMain.Rcon.ExecuteCommandAsync($"function {testFunctionScope.McFunctionPath}");

        Context = new McfTestContext(TestMain.Rcon, testFunctionScope);
        
        // wait for the test to complete
        await Context.WaitForCompletion();
    }

    private static async Task SetVariableValue(Variable target, object value)
    {
        string mcfValue = target.ToMcfValue(value);

        string command;
        if (target.Location.DataLocation == DataLocation.Scoreboard)
        {
            command = $"scoreboard players set {target.Location} {mcfValue}";
        }
        else
        {
            command = $"data modify storage amethyst {target.Location} set value {mcfValue}";
        }

        await TestMain.Rcon.ExecuteCommandAsync(command);
    }
}