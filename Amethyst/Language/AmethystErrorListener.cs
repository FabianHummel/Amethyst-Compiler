using Antlr4.Runtime;

namespace Amethyst;

public class AmethystErrorListener : BaseErrorListener
{
    private Parser Parser { get; }
    
    public AmethystErrorListener(Parser parser)
    {
        Parser = parser;
    }
    
    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new SyntaxException(msg, line, charPositionInLine, offendingSymbol.InputStream.SourceName!);
    }
}