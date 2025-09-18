using Antlr4.Runtime;

namespace Amethyst.Model;

public class LoopingScope : IDisposable
{
    private readonly Compiler _compiler;
    private readonly LoopingScope _previousScope;
    
    public bool IsCancelled { get; private set; } = false;
    
    public LoopingScope(Compiler compiler)
    {
        _compiler = compiler;
        _previousScope = compiler.LoopingScope;
    }

    public void Break()
    {
        IsCancelled = true;
    }

    public void Dispose()
    {
        _compiler.LoopingScope = _previousScope;
        
        GC.SuppressFinalize(this);
    }
}