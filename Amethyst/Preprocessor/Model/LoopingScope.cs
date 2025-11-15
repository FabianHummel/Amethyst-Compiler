using System.Diagnostics.CodeAnalysis;

namespace Amethyst.Model;

/// <summary>A helper class to manage looping scopes in the preprocessor. Allows for breaking out of
/// loops via the <see cref="Break" /> method. The current looping scope is referenced
/// <see cref="Compiler.LoopingScope">here</see>.</summary>
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

    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
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