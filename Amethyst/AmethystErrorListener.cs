using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public class AmethystErrorListener : BaseErrorListener
{
    private Namespace Context { get; }
    private string FileName { get; }
    
    public AmethystErrorListener(Namespace context, string fileName)
    {
        Context = context;
        FileName = fileName;
    }
    
    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new SyntaxException(msg, line, charPositionInLine, Path.Combine(Context.SourceFilePath, FileName));
    }
}