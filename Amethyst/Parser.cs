using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Constants;
using Antlr4.Runtime;

namespace Amethyst;

public class Parser
{
    private string Source { get; }
    public string FilePath { get; }
    public Context Context { get; }
    public Namespace? Ns { get; set; }
    
    public Parser(Context context, string source, string filePath, Namespace? ns)
    {
        Source = source;
        FilePath = filePath;
        Context = context;
        Ns = ns;
    }
    
    public AmethystParser.FileContext Parse(out Namespace ns)
    {
        var inputStream = new AntlrInputStream(Source);
        var lexer = new AmethystLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new AmethystParser(tokenStream);
        parser.AddErrorListener(new AmethystErrorListener(this));
        parser.AddParseListener(new AmethystParseListener(this));
        var tree = parser.file();
        ns = Ns ?? throw new SyntaxException($"Namespace not defined. Either use the 'namespace' keyword or place the file in a directory within '/{SOURCE_DIRECTORY}'.", 0, 0, FilePath);
        return tree;
    }
}