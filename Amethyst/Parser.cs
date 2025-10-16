using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public class Parser
{
    public SourceFile SourceFile { get; set; } = null!;
    
    public void Parse(SourceFile sourceFile, string path)
    {
        var stream = File.OpenRead(path);
        Parse(sourceFile, stream);
    }

    public void Parse(SourceFile sourceFile, Stream fileStream)
    {
        SourceFile = sourceFile;
        var inputStream = new AmethystInputStream(fileStream, sourceFile);
        var lexer = new AmethystLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new AmethystParser(tokenStream);
        parser.AddErrorListener(new AmethystErrorListener(this));
        parser.AddParseListener(new AmethystParseListener(this));
        parser.file();
    }
}