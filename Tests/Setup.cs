using System.Diagnostics;
using System.Net.Sockets;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using RconSharp;

namespace Tests;

[SetUpFixture]
public static partial class TestMain
{
    private static IEnumerable<ITest> GetDescendants(ITest test)
    {
        return test.Tests.Concat(test.Tests.SelectMany(GetDescendants));
    }

    [OneTimeSetUp]
    public static async Task Setup()
    {
        var worldPath = Path.Combine(Environment.CurrentDirectory, "world");
        if (Directory.Exists(worldPath))
        {
            Directory.Delete(worldPath, true);
        }
        
        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "world/datapacks"));

        foreach (var test in GetDescendants(TestExecutionContext.CurrentContext.CurrentTest))
        {
            CreateDatapackForTest(test);
        }
        
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

        connect:
        try
        {
            _rcon = RconClient.Create(_settings.RconHost, _settings.RconPort);
            await _rcon.ConnectAsync();
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            Thread.Sleep(1000);
            goto connect;
        }

        if (!await _rcon.AuthenticateAsync(_settings.RconPassword))
        {
            await Console.Error.WriteLineAsync("Failed to authenticate with RCON server.");
            throw new Exception("Failed to authenticate with RCON server.");
        }
    }
    
    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await _rcon.ExecuteCommandAsync("stop");
    }
}