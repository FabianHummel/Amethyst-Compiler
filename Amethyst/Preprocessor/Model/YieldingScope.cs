using Antlr4.Runtime;

namespace Amethyst.Model;

public class YieldingScope : IDisposable
{
    private readonly Compiler _compiler;
    private readonly YieldingScope _previousScope;
    private readonly Type _allowedType;
    
    private readonly List<ParserRuleContext> _result = new();

    public IReadOnlyList<ParserRuleContext> Result => _result;

    public YieldingScope(Compiler compiler, Type allowedType)
    {
        _compiler = compiler;
        _allowedType = allowedType;
        _previousScope = compiler.YieldingScope;
    }

    public void Yield(ParserRuleContext context)
    {
        if (!_allowedType.IsInstanceOfType(context))
        {
            throw new SyntaxException($"Cannot yield '{context.GetType().Name}' in this context. Expected '{_allowedType.Name}'.", context);
        }

        _result.Add(context);
    }

    public void Dispose()
    {
        _compiler.YieldingScope = _previousScope;
        
        GC.SuppressFinalize(this);
    }
}