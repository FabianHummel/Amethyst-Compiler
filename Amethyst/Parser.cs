using Amethyst.Language;
using Amethyst.Model;
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
    
    public AmethystParser.FileContext Parse()
    {
        var inputStream = new AntlrInputStream(Source);
        var lexer = new AmethystLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new AmethystParser(tokenStream);
        parser.AddErrorListener(new AmethystErrorListener(this));
        parser.AddParseListener(new AmethystParseListener(this));
        return parser.file();
    }
}