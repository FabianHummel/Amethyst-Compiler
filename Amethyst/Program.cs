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
            
            var compileTargets = Project.FindCompileTargets(Environment.CurrentDirectory);
        
            foreach (var target in compileTargets)
            {
                Console.Out.WriteLine("Compiling " + target);
                var input = File.ReadAllText(target);
                var tokens = Lexer.Tokenize(input).ToList(); 
                /* var ast = */ Parser.Parse(tokens);
            }
        }
    }
}