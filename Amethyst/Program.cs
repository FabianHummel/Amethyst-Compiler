namespace Amethyst;

using Tommy;

internal static class Program
{
    private static void Main(string[] args)
    {
        using (var reader = File.OpenText("amethyst.toml"))
        {
            var table = TOML.Parse(reader);

            var outDir = table["output"].AsString;
            Project.RegenerateOutputFolder(outDir);
            Project.CreateMcMeta(outDir, table);
            Project.CreateDataFolders(outDir, table);
            
            var rootNamespace = table["namespace"].AsString;
            
            var compileTargets = Project.FindCompileTargets(Environment.CurrentDirectory);
        
            foreach (var target in compileTargets)
            {
                Console.Out.WriteLine("Compiling " + target);
                var input = File.ReadAllText(target);
                var tokens = Lexer.Tokenize(input);
                var nodes = Parser.ParseBody(tokens);
                Generator.Generate(nodes, rootNamespace, outDir);
            }
        }
    }
}