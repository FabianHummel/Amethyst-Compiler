using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public class AmethystErrorListener : BaseErrorListener
{
    private Namespace Context { get; }
    
    public AmethystErrorListener(Namespace context)
    {
        Context = context;
    }
    
    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new SyntaxException(msg, line, charPositionInLine, Context.SourceFilePath);
    }
}