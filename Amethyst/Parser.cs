using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public class Parser
{
    public string? RegistryName { get; set; }
    public SourceFile? SourceFile { get; set; }
    
    public AmethystParser.FileContext Parse(SourceFile sourceFile)
    {
        SourceFile = sourceFile;
        var stream = File.OpenRead(sourceFile.Path);
        var inputStream = new AmethystInputStream(stream, sourceFile);
        var lexer = new AmethystLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new AmethystParser(tokenStream);
        parser.AddErrorListener(new AmethystErrorListener(this));
        parser.AddParseListener(new AmethystParseListener(this));
        return parser.file();
    }
}