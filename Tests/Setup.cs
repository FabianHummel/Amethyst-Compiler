using System.Diagnostics;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Amethyst;
using Fluid;
using NUnit.Framework;
using RconSharp;
using Tests.Presets;

namespace Tests;

[SetUpFixture]
public static partial class TestMain
{
    private static readonly FluidParser _parser = new();
    
    [OneTimeSetUp]
    public static async Task SetupTestEnvironment()
    {
        ResetServerWorld();
        ExpandFluidTemplates();
        CompileAmethystDatapack();
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

    private static void ExpandFluidTemplates()
    {
        var model = new
        {
            ArithmeticOperators = ArithmeticOperatorTestCase.Preset,
            DefaultNumericTypes = NumericTypeTestCase.DefaultPreset,
            AllNumericTypes = NumericTypeTestCase.AllPreset,
            RegularStrings = StringTestCase.Regular,
            InterpolatedStrings = StringTestCase.Interpolated,
        };

        var context = new TemplateContext(model, new TemplateOptions
        {
            MemberAccessStrategy = new UnsafeMemberAccessStrategy
            {
                MemberNameStrategy = MemberNameStrategies.SnakeCase
            }
        });

        foreach (var liquidTemplatePath in Directory.GetFiles("datapack", "*.liquid", SearchOption.AllDirectories))
        {
            var templateContent = File.ReadAllText(liquidTemplatePath);
            var template = _parser.Parse(templateContent);
            var renderedContent = template.Render(context);

            var outputPath = Regex.Replace(liquidTemplatePath, @"\.liquid$", "");
            File.WriteAllText(outputPath, renderedContent);
            File.Delete(liquidTemplatePath);
        }
    }
    
    private static void CompileAmethystDatapack()
    {
        Amethyst = new Processor("datapack", 0, rethrowErrors: true);
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
            throw new InvalidOperationException("Failed to authenticate with RCON server.");
        }
    }
}