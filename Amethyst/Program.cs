﻿namespace Amethyst;

using Tommy;

internal static class Program
{
    private static void Main(string[] args)
    {
        using (var reader = File.OpenText("amethyst.toml"))
        {
            var table = TOML.Parse(reader);

            var outDir = (string)table["output"].AsString;
            Project.RegenerateOutputFolder(outDir);
            Project.CreateMcMeta(outDir, table);
            
            outDir = Path.Combine(outDir, "data");
            Project.CreateDataFolders(outDir, table);
            
            var rootNamespace = table["namespace"].AsString;
            
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