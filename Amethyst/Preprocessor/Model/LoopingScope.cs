using Antlr4.Runtime;

namespace Amethyst.Model;

public class LoopingScope : IDisposable
{
    private class BreakException : Exception
    {
        
    }

    private readonly Compiler _compiler;
    private readonly LoopingScope _previousScope;
    
    public LoopingScope(Compiler compiler)
    {
        _compiler = compiler;
        _previousScope = compiler.LoopingScope;
    }

    public void Run(Action cb)
    {
        try
        {
            cb();
        }
        catch (BreakException ex)
        {
            return;
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