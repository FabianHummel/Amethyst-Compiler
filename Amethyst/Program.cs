using System.Collections;
using System.Runtime.InteropServices;

namespace Amethyst;

using Tommy;

internal static class Program
{
    private static void Main(string[] args)
    {
        using (var reader = File.OpenText("amethyst.toml"))
        {
            var table = TOML.Parse(reader);

            var minecraftRoot = new Func<string>(() =>
            {
                if (table["minecraft"].AsString is {} dir)
                {
                    return dir;
                }
                
                if (System.Environment.OSVersion.Platform == PlatformID.Win32NT ||
                    System.Environment.OSVersion.Platform == PlatformID.Win32S ||
                    System.Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                    System.Environment.OSVersion.Platform == PlatformID.WinCE)
                {
                    return System.Environment.ExpandEnvironmentVariables("%APPDATA%\\.minecraft");
                }

                if (System.Environment.OSVersion.Platform == PlatformID.MacOSX ||
                    System.Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Library/Application Support/minecraft");
                }

                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".minecraft");
            })();

            var minecraftRootExists = Directory.Exists(minecraftRoot);
            
            if (!minecraftRootExists)
            {
                Console.Error.WriteLine($"Minecraft root not found at {minecraftRoot}, using current directory as root");
            }

            var name = table["name"].AsString ?? throw new Exception("Name not configured");
            
            var outDir = new Func<string>(() =>
            {
                var outDir = (string?)table["output"].AsString;
                if (minecraftRootExists && outDir == null)
                {
                    return Path.Combine(minecraftRoot, "datapacks");
                }
                if (!minecraftRootExists || outDir!.StartsWith("."))
                {
                    return Path.Combine(System.Environment.CurrentDirectory, outDir!);
                }
                if (outDir.StartsWith("/"))
                {
                    return outDir;
                }
                
                return Path.Combine(minecraftRoot, "saves", outDir, "datapacks", name);
            })();
            
            Project.RegenerateOutputFolder(outDir);
            Project.CreateMcMeta(outDir, table);
            Project.CreateDataFolders(outDir, table);

            var rootNamespace = table["namespace"].AsString ?? throw new Exception("Namespace not configured in amethyst.toml");
            
            var compileTargets = Project.FindCompileTargets(System.Environment.CurrentDirectory);
        
            try
            {
                foreach (var target in compileTargets)
                {
                    Console.Out.WriteLine("Compiling " + target);
                    var input = File.ReadAllText(target);
                    var tokenizer = new Tokenizer(input);
                    var tokens = tokenizer.ScanTokens();
                    var parser = new Parser(tokens);
                    var stmts = parser.Parse();
                    var optimizer = new Optimizer(stmts);
                    stmts = optimizer.Optimize();
                    var compiler = new Compiler(stmts, rootNamespace, outDir);
                    compiler.Compile();
                }
            }
            catch (SyntaxException e)
            {
                Console.Error.WriteLine(e.Message + " at line " + e.Line);
            }
        }
    }
}