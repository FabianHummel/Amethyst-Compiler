using System.Text.RegularExpressions;
using NUnit.Framework;
using Amethyst;
using Brigadier.NET.Exceptions;
using Newtonsoft.Json.Linq;

namespace Tests;

public partial class Program
{
    private readonly AmethystDispatcher _dispatcher = new();
    
    private readonly Processor _amethyst = new();

    private Dictionary<string, Dictionary<string, int>> Scoreboard => _dispatcher.Scoreboard;

    private static string GetFunctionPath(string mcFunctionPath)
    {
        var paths = mcFunctionPath.Split(":", 2);
        return Path.Combine(Environment.CurrentDirectory, "output/test/data", paths[0], "functions", paths[1] + ".mcfunction");
    }
    
    [GeneratedRegex("#.*")]
    private static partial Regex CommentRegex();
    
    private void SanitizeAndDispatch(string code)
    {
        foreach (var se in code.Split("\n"))
        {
            var line = se.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            if (CommentRegex().IsMatch(line))
            {
                continue;
            }
            try
            {
                _dispatcher.Execute(line, null);
            }
            catch (CommandSyntaxException e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
    
    public void Run(string input)
    {
        var mainAmyFile = Path.Combine(Environment.CurrentDirectory, "src/test/main.amy");
        File.WriteAllText(mainAmyFile, input);
        _amethyst.ReinitializeProject();
        
        var loadJsonFile = Path.Combine(Environment.CurrentDirectory, "output/test/data/minecraft/tags/functions/load.json");
        dynamic load = JObject.Parse(File.ReadAllText(loadJsonFile));
        
        foreach (string mcFunctionPath in load.values)
        {
            var path = GetFunctionPath(mcFunctionPath);
            var function = File.ReadAllText(path);
            SanitizeAndDispatch(function);
        }
    }

    [SetUp]
    public void Setup()
    {
        _dispatcher.Scoreboard.Clear();
    }
}