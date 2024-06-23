using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public class Parser
{
    private string Source { get; }
    private Namespace Context { get; }
    
    public Parser(string source, Namespace context)
    {
        Source = source;
        Context = context;
    }
    
    public AmethystParser.FileContext Parse()
    {
        var inputStream = new AntlrInputStream(Source);
        var lexer = new AmethystLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new AmethystParser(tokenStream);
        parser.AddErrorListener(new AmethystErrorListener(Context));
        parser.AddParseListener(new AmethystParseListener(Context));
        return parser.file();
    }
}