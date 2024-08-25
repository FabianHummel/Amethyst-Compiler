using System.Diagnostics;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using NUnit.Framework;
using Amethyst;
using Microsoft.Extensions.Configuration;
using Tests.RCON;

namespace Tests;

public partial class Program
{
    private readonly Processor _amethyst = new();
    private readonly MinecraftServerSettings _settings = new();
    private readonly Process _process;
    private readonly MinecraftClient _rcon;

    public Program()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();
        
        configuration.GetSection("MinecraftServer").Bind(_settings);
        
        var serverProperties = Path.Combine(Environment.CurrentDirectory, "server.properties");
        File.WriteAllText(serverProperties, File.ReadAllText(serverProperties, Encoding.UTF8)
            .Replace("{RCON_PASSWORD}", _settings.RconPassword)
            .Replace("{RCON_PORT}", _settings.RconPort.ToString()));
        
        var serverStartInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-Xmx1024M -Xms1024M -jar \"{_settings.JarPath}\" nogui",
            WorkingDirectory = Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = serverStartInfo };
        _process.Start();

        connect:
        try
        {
            _rcon = new MinecraftClient(_settings.RconHost, _settings.RconPort);
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            Thread.Sleep(1000);
            goto connect;
        }

        if (!_rcon.Authenticate(_settings.RconPassword))
        {
            Console.Error.WriteLine("Failed to authenticate with RCON server.");
            throw new Exception("Failed to authenticate with RCON server.");
        }
    }
    
    public void Run(string input)
    {
        var mainAmyFile = Path.Combine(Environment.CurrentDirectory, "src/test/main.amy");
        File.WriteAllText(mainAmyFile, input);
        _amethyst.ReinitializeProject();
        
        var sourcePath = Path.Combine(Environment.CurrentDirectory, "output/test");
        var targetPath = Path.Combine(Environment.CurrentDirectory, "world/datapacks/test.zip");

        using (var writer = new FileStream(targetPath, FileMode.Create))
        {
            ZipFile.CreateFromDirectory(sourcePath, writer, CompressionLevel.Fastest, false);
        }
        
        _rcon.SendCommand("reload", out _);
    }

    private int GetScoreboardValue(string objective, string target)
    {
        _rcon.SendCommand($"scoreboard players get {target} {objective}", out var msg);
        // "<target> has <value> [<objective>]"
        return int.Parse(msg.Body.Split("has ")[1].Split(" ")[0]);
    }

    private string GetStorageValue(string @namespace, string path)
    {
        _rcon.SendCommand($"data get storage {@namespace} {path}", out var msg);
        // Storage <namespace> has the following contents: <contents>
        return msg.Body.Split("contents: ")[1].Trim();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _rcon.SendCommand("stop", out _);
    }
}