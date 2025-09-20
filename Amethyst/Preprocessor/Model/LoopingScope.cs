namespace Amethyst.Model;

public class LoopingScope : IDisposable
{
    private class BreakException : Exception;

    private readonly Compiler _compiler;
    private readonly LoopingScope _previousScope;
    
    public LoopingScope(Compiler compiler, Action cb)
    {
        _compiler = compiler;
        _previousScope = compiler.LoopingScope;
        compiler.LoopingScope = this;
        
        try
        {
            cb();
        }
        catch (BreakException)
        {
            // Swallow
        }
    }

    public void Break()
    {
        throw new BreakException();
    }

    public void Dispose()
    {
        _compiler.LoopingScope = _previousScope;
        
        GC.SuppressFinalize(this);
    }
}