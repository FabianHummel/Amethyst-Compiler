using System.Text.RegularExpressions;
using Amethyst;
using Amethyst.Model;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Tests;

[TestFixture]
public abstract class ServerTestBase
{
    protected McfTestContext Context { get; private set; } = null!;

    [SetUp]
    public async Task Setup()
    {
        var currentTest = TestExecutionContext.CurrentContext.CurrentTest;

        Console.SetOut(TestExecutionContext.CurrentContext.OutWriter);

        var linkAttribute = currentTest.Method!
            .GetCustomAttributes<LinkAttribute>(inherit: false)
            .First();
        
        if (linkAttribute == null)
        {
            throw new InvalidOperationException($"Test method '{currentTest.MethodName}' must have a Link attribute when tested on a server.");
        }
        
        var linkPath = ExpandLinkPath(linkAttribute, currentTest);
        
        if (!TestMain.Amethyst.Context.UnitTests.TryGetValue(linkPath, out var testFunctionScope))
        {
            throw new InvalidOperationException($"Test method '{currentTest.MethodName}' could not be linked to function in Amethyst code base.");
        }
        
        // setup function arguments by sending commands to the server
        var methodParameters = currentTest.Method!
            .GetParameters()
            .Where(p => p.IsDefined<FunctionParameterAttribute>(inherit: false))
            .ToArray();

        for (var index = 0; index < methodParameters.Length; index++)
        {
            var methodParameter = methodParameters[index];
            var parameterName = methodParameter.ParameterInfo.Name;
            if (!testFunctionScope.TryGetSymbol(parameterName!, out var parameterSymbol) || parameterSymbol is not Variable parameterVariable)
            {
                throw new InvalidOperationException($"Test function '{linkPath}' does not have a parameter '{parameterName}'.");
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

    private static string ExpandLinkPath(LinkAttribute linkAttribute, ITest test)
    {
        var methodParameters = test.Method!
            .GetParameters();

        var matches = Regex.Matches(linkAttribute.Path, "{(.*?)}")
            .Select(m => m.Groups[1].Value);

        var resultPath = linkAttribute.Path;
            
        foreach (var match in matches)
        {
            var objectPath = match.Split('.');
            var parameterName = objectPath[0];
            var parameterIndex = Array.FindIndex(methodParameters, p => p.ParameterInfo.Name == parameterName);
            if (parameterIndex == -1)
            {
                throw new InvalidOperationException($"Parameter '{parameterName}' not found in method '{test.Method.Name}'.");
            }
                
            var parameterValue = test.Arguments[parameterIndex];
                
            for (int i = 1; i < objectPath.Length && parameterValue != null; i++)
            {
                var prop = parameterValue.GetType().GetProperty(objectPath[i]);
                if (prop == null)
                {
                    throw new InvalidOperationException($"Property '{objectPath[i]}' not found on '{parameterValue.GetType().Name}'.");
                }
                parameterValue = prop.GetValue(parameterValue);
            }
                
            resultPath = resultPath.Replace("{" + match + "}", parameterValue?.ToString());
        }
        
        return resultPath;
    }
    
    private static async Task SetVariableValue(Variable target, object value)
    {
        string mcfValue = ToMcfValue(target, value);

        string command;
        if (target.Location.DataLocation == DataLocation.Scoreboard)
        {
            command = $"scoreboard players set {target.Location} {mcfValue}";
        }
        else
        {
            command = $"data modify storage amethyst {target.Location} set value {mcfValue}";
        }

        TestContext.WriteLine($"/{command}");
        await TestMain.Rcon.ExecuteCommandAsync(command);
    }
    
    private static string ToMcfValue(Variable target, object value)
    {
        if (target.Location.DataLocation == DataLocation.Scoreboard)
        {
            if (target.Datatype is DecimalDatatype decimalDatatype)
            {
                value = Math.Round((double)value, decimalDatatype.DecimalPlaces);
            }
            
            if (!IScoreboardValue.TryParse(value, out var scoreboardValue))
            {
                throw new InvalidOperationException($"Cannot convert value '{value}' to scoreboard value.");
            }
            return scoreboardValue.ScoreboardValue.ToString();
        }

        if (target.Location.DataLocation == DataLocation.Storage)
        {
            if (!IConstantValue.TryParse(value, out var storageValue))
            {
                throw new InvalidOperationException($"Cannot convert value '{value}' to storage value.");
            }
            return storageValue.ToNbtString();
        }

        throw new InvalidOperationException($"Unknown data location '{target.Location.DataLocation}'.");
    }
}