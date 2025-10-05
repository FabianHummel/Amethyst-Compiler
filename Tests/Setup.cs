using System.Diagnostics;
using System.Net.Sockets;
using Amethyst;
using NUnit.Framework;
using RconSharp;

namespace Tests;

[SetUpFixture]
public static partial class TestMain
{
    [OneTimeSetUp]
    public static async Task SetupTestEnvironment()
    {
        ResetServerWorld();
        Amethyst = new Processor("datapack", 0, rethrowErrors: true);
        StartServer();
        await ConnectToServerRcon();
    }
    
    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await Rcon.ExecuteCommandAsync("stop");
    }

    private static void ResetServerWorld()
    {
        var worldPath = Path.Combine(Environment.CurrentDirectory, "world");
        if (Directory.Exists(worldPath))
        {
            Directory.Delete(worldPath, true);
        }
        
        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "world/datapacks"));
    }

    private static void StartServer()
    {
        var serverStartInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-Xmx1024M -Xms1024M -jar \"{_settings.JarPath}\"",
            WorkingDirectory = Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = serverStartInfo };
        _process.Start();
        _process.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);
        _process.BeginOutputReadLine();
    }

    private static async Task ConnectToServerRcon()
    {
        connect:
        try
        {
            Rcon = RconClient.Create(_settings.RconHost, _settings.RconPort);
            await Rcon.ConnectAsync();
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            Thread.Sleep(1000);
            goto connect;
        }

        if (!await Rcon.AuthenticateAsync(_settings.RconPassword))
        {
            await Console.Error.WriteLineAsync("Failed to authenticate with RCON server.");
            throw new Exception("Failed to authenticate with RCON server.");
        }
    }
}