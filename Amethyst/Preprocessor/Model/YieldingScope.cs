using Antlr4.Runtime;

namespace Amethyst.Model;

/// <summary>A helper class to manage yielding statements in the preprocessor. The <c>YIELD</c> keyword
/// produces a value inside an expression without immediately returning from the function. The
/// operation acts like an inline value generator—when evaluated, it inserts the yielded value into the
/// surrounding expression. The example is a very basic use case of yield, although it should be used
/// in big constructs like generating entire resources. The current yielding scope is reference
/// <see cref="Compiler.YieldingScope">here</see>.</summary>
/// <example>
///     <code>
/// VAR X = 123;
/// var parts = ["X", "is", IF(X%2==0){ YIELD "even"; } ELSE { YIELD "odd"; }];</code>
/// </example>
public class YieldingScope : IDisposable
{
    private readonly Compiler _compiler;
    private readonly YieldingScope _previousScope;
    private readonly Type _allowedType;
    
    private readonly List<object> _result = new();

    public IReadOnlyList<object> Result => _result;

    public YieldingScope(Compiler compiler, Type allowedType)
    {
        _compiler = compiler;
        _allowedType = allowedType;
        _previousScope = compiler.YieldingScope;
        compiler.YieldingScope = this;
    }

    public void Yield(object result, ParserRuleContext context)
    {
        if (!_allowedType.IsInstanceOfType(result))
        {
            throw new SyntaxException($"Cannot yield '{result.GetType().Name}' in this context. Expected '{_allowedType.Name}'.", context);
        }

        _result.Add(result);
    }

    public void Dispose()
    {
        _compiler.YieldingScope = _previousScope;
        
        GC.SuppressFinalize(this);
    }
}